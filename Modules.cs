using Newtonsoft.Json;
using SkiaSharp;
using System.Drawing.Text;
using System.Text;
using System.Text.RegularExpressions;

namespace mylabel.Modules
{
    public static class Modules
    {
        //public static void GPUDrawAnnotations(SKCanvas canvas, ImageInfo currentImage, string currentImageName,
        //                              SKSize imgSize, float scale, ImageLabel selectedLabel, float imageDpi)
        //{
        //    if (currentImage == null || currentImage.Labels == null) return;

        //    foreach (var label in currentImage.Labels)
        //    {
        //        float x = (float)(label.Position.X * imgSize.Width);
        //        float y = (float)(label.Position.Y * imgSize.Height);

        //        bool isSelected = (label == selectedLabel);
        //        SKColor themeColor = isSelected ? SKColors.Purple : SKColors.Red;

        //        using (var paint = new SKPaint())
        //        {
        //            paint.IsAntialias = true;
        //            paint.Color = themeColor;
        //            paint.SubpixelText = true;

        //            // 1. 绘制序号
        //            float indexFontSize = 12f * imageDpi;
        //            using (var indexTypeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold))
        //            using (var indexFont = new SKFont(indexTypeface, indexFontSize))
        //            {
        //                string idxStr = label.Index.ToString();
        //                SKRect idxBounds = new SKRect();
        //                indexFont.MeasureText(idxStr, out idxBounds, paint);
        //                // 更加精准的居中对齐
        //                canvas.DrawText(idxStr, x - idxBounds.MidX, y - idxBounds.MidY, indexFont, paint);
        //            }

        //            // 2. 优化后的竖排文字绘制
        //            if (!string.IsNullOrEmpty(label.Text))
        //            {
        //                float currentOriginFontSize = label.FontSize > 0 ? (float)label.FontSize : 12f;
        //                float fontSize = currentOriginFontSize * imageDpi;

        //                // 【关键：动态加载字体】
        //                // 如果 label.FontFamily 无效，Skia 会自动 fallback 到系统默认
        //                using (var typeface = SKTypeface.FromFamilyName(label.FontFamily, SKFontStyle.Normal))
        //                using (var textFont = new SKFont(typeface, fontSize))
        //                {
        //                    // 设置行高倍数（1.2倍通常比较美观）
        //                    float lineHeight = fontSize * 1.2f;
        //                    // 文字起始位置：x 坐标向右偏移序号宽度的一半，再加点留白
        //                    float startX = x + (indexFontSize * 0.8f);
        //                    float currentY = y;

        //                    foreach (char c in label.Text)
        //                    {
        //                        string charStr = c.ToString();

        //                        // 测量当前字符的尺寸，用于精细对齐
        //                        SKRect charBounds = new SKRect();
        //                        textFont.MeasureText(charStr, out charBounds, paint);

        //                        // 居中绘制字符：startX 是中心轴
        //                        // 这样即使混排了不同宽度的字符（如 'i' 和 'W'），也能在垂直线上对齐
        //                        float charX = startX - charBounds.MidX;

        //                        // 绘制（注意：Skia 默认 Y 是基线，所以要加上文字高度的一部分）
        //                        canvas.DrawText(charStr, charX, currentY + fontSize * 0.85f, textFont, paint);

        //                        // 纵向步进
        //                        currentY += lineHeight;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        /// <summary>
        /// 核心绘图函数：绘制序号及竖向文字
        /// </summary>
        public static void DrawAnnotations(Graphics g, ImageInfo currentImage,Size imgSize, 
                                                 float scale, ImageLabel selectedLabel, float imageDpi, bool onlyShowIndex = false)
        {
            // 如果当前图片对象为空，直接退出
            if (currentImage == null || currentImage.Labels == null) return;
            // 开启抗锯齿，保证缩放后文字依然清晰
            g.TextRenderingHint = TextRenderingHint.AntiAlias;


            // 准备竖排格式
            using (StringFormat vFormat = new StringFormat())
            {
                vFormat.FormatFlags = StringFormatFlags.DirectionVertical | StringFormatFlags.DirectionRightToLeft;
                vFormat.Alignment = StringAlignment.Near;

                foreach (var label in currentImage.Labels)
                {

                    // 计算在图片上的实际像素坐标
                    float x = label.Position.X * imgSize.Width;
                    float y = label.Position.Y * imgSize.Height;

                    // 颜色逻辑
                    bool isSelected = (label == selectedLabel);
                    Brush themeBrush = isSelected ? Brushes.Purple : Brushes.Red;
                    // --- A. 绘制序号 (始终绘制) ---
                    using (Font indexFont = new Font("Arial", 20 * imageDpi, FontStyle.Bold, GraphicsUnit.Pixel))
                    {
                        string idxStr = label.Index.ToString();
                        SizeF size = g.MeasureString(idxStr, indexFont);
                        float centerX = x - (size.Width / 2f);
                        float centerY = y - (size.Height / 2f);

                        g.DrawString(idxStr, indexFont, themeBrush, centerX, centerY);

                        // --- B. 绘制内容 (受 onlyShowIndex 控制) ---
                        // 如果开启了“仅显示序号”，或者文本为空，则跳过以下逻辑
                        if (onlyShowIndex || string.IsNullOrEmpty(label.Text)) continue;

                        // 只有非文校模式才创建这些字体，节省性能
                        float currentOriginFontSize = label.FontSize > 0 ? (float)label.FontSize : 12f;
                        float currentFontSize = currentOriginFontSize * imageDpi;

                        Font textFont = null;
                        try
                        {
                            textFont = new Font(label.FontFamily, currentFontSize, FontStyle.Regular, GraphicsUnit.Pixel);
                        }
                        catch
                        {
                            textFont = new Font("Microsoft YaHei", currentFontSize, FontStyle.Regular, GraphicsUnit.Pixel);
                        }

                        using (textFont)
                        {
                            float textX = centerX + (currentFontSize * 0.4f);
                            g.DrawString(label.Text, textFont, themeBrush, textX, y, vFormat);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 将特定格式的文本解析为 imageLabel 列表
        /// </summary>
        public static Dictionary<string, ImageInfo> ParseTextToLabels(string content)
        {
            // 改为返回字典
            var database = new Dictionary<string, ImageInfo>();
            string[] lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            string currentImgName = string.Empty;
            ImageLabel currentLabel = null;

            var imgRegex = new Regex(@">>>>>>>>\[(.*?)\]<<<<<<<<");
            var metaRegex = new Regex(@"----------------\[(\d+)\]----------------\[([\d\.]+),([\d\.]+),(\d+)\]");

            foreach (string rawLine in lines)
            {
                string line = rawLine.Trim();
                if (string.IsNullOrEmpty(line)) continue;

                // 1. 识别图片行
                var imgMatch = imgRegex.Match(line);
                if (imgMatch.Success)
                {
                    currentImgName = imgMatch.Groups[1].Value;
                    if (!database.ContainsKey(currentImgName))
                    {
                        database[currentImgName] = new ImageInfo { ImageName = currentImgName };
                    }
                    continue;
                }

                // 2. 识别元数据行
                var metaMatch = metaRegex.Match(line);
                if (metaMatch.Success)
                {
                    // 如果图片名还没识别到就出现了元数据，跳过或处理异常
                    if (string.IsNullOrEmpty(currentImgName)) continue;

                    int index = int.Parse(metaMatch.Groups[1].Value);
                    float x = float.Parse(metaMatch.Groups[2].Value);
                    float y = float.Parse(metaMatch.Groups[3].Value);
                    int groupValue = int.Parse(metaMatch.Groups[4].Value);

                    currentLabel = new ImageLabel
                    {
                        Index = index, // 使用文件中的索引
                        Position = new BoundingBox { X = x, Y = y, Width = 0f, Height = 0f },
                        Group = (groupValue == 1 ? "框内" : "框外"),
                        Text = ""
                    };

                    // 直接添加到对应的 ImageInfo 中
                    database[currentImgName].Labels.Add(currentLabel);
                    continue;
                }

                // 3. 识别文本行
                if (currentLabel != null && !line.StartsWith("-") && !line.StartsWith(">"))
                {
                    if (string.IsNullOrEmpty(currentLabel.Text))
                        currentLabel.Text = line;
                    else
                        currentLabel.Text += Environment.NewLine + line;
                }
            }

            return database;
        }

        /// <summary>
        /// 将列表数据转换回特定的 txt 文本格式
        /// </summary>
        public static string LabelsToText(Dictionary<string, ImageInfo> imageDatabase)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            // 1. 写入固定的头部信息
            sb.AppendLine("1,0");
            sb.AppendLine("-");
            sb.AppendLine("框内");
            sb.AppendLine("框外");
            sb.AppendLine("-");
            sb.AppendLine("Default Comment");
            sb.AppendLine("You can edit me");
            sb.AppendLine();
            sb.AppendLine();

            // 2. 字典的 Keys 已经是唯一的图片名，直接排序后遍历
            var sortedImageNames = imageDatabase.Keys.OrderBy(name => name);

            foreach (var imageName in sortedImageNames)
            {
                var imageInfo = imageDatabase[imageName];

                // 写入图片分隔符（使用 Path.GetFileName 确保格式统一）
                string pureName = System.IO.Path.GetFileName(imageName);
                sb.AppendLine($">>>>>>>>[{pureName}]<<<<<<<<");

                // 3. 遍历该图片下的所有标注（已经由 ImageInfo 管理，直接排序 Index 即可）
                foreach (var label in imageInfo.Labels.OrderBy(l => l.Index))
                {
                    // 保持原有的 Group 逻辑
                    int groupValue = label.Group == "框内" ? 1 : 2;

                    // 写入标注行
                    sb.AppendLine($"----------------[{label.Index}]----------------[{label.Position.X:F3},{label.Position.Y:F3},{groupValue}]");

                    // 写入文本内容
                    sb.AppendLine(label.Text);
                    sb.AppendLine();
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        /// <summary>
        /// 识别字体函数
        /// </summary>
        public static async Task<string> GetFontRecognitionResult(string imagePath)
        {
            using (var client = new HttpClient())
            {
                // 设置超时时间，防止 Docker 没启动时程序卡死
                client.Timeout = TimeSpan.FromSeconds(10);
                var url = "http://localhost:7860/api/predict";

                byte[] imageBytes = File.ReadAllBytes(imagePath);
                string base64Image = "data:image/png;base64," + Convert.ToBase64String(imageBytes);

                var requestBody = new { data = new[] { base64Image } };
                var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

                try
                {
                    var response = await client.PostAsync(url, content);
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        dynamic resultObj = JsonConvert.DeserializeObject(jsonResponse);

                        // 根据 demo.py，第一个返回项通常是 Label 概率字典
                        // 我们取概率最高（第一个）的 Key
                        var predictions = resultObj.data[0];
                        string bestMatch = "未知字体";

                        // 这里的解析逻辑取决于 Gradio 具体的返回格式
                        // 通常是一个 Dictionary<string, double>
                        foreach (var prop in predictions)
                        {
                            bestMatch = prop.Name; // 拿到第一个 Key 就跳出
                            break;
                        }
                        return bestMatch;
                    }
                }
                catch (Exception)
                {
                    return "本地 Docker 未响应";
                }
            }
            return "识别未成功";
        }


    }
}
