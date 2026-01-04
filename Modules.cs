using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Text;
using System.Text.RegularExpressions;

namespace mylabel.Modules
{
    public static class Modules
    {
        /// <summary>
        /// 核心绘图函数：绘制序号及竖向文字
        /// </summary>
        public static void DrawAnnotations(Graphics g, List<imageLabel> labels, string currentImageName,
                                                 Size imgSize, float scale, int selectedIndex)
        {
            // 开启抗锯齿，保证缩放后文字依然清晰
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            // 准备竖排格式
            using (StringFormat vFormat = new StringFormat())
            {
                vFormat.FormatFlags = StringFormatFlags.DirectionVertical| StringFormatFlags.DirectionRightToLeft;
                vFormat.Alignment = StringAlignment.Near;

                foreach (var label in labels)
                {
                    if (label.ImageName != currentImageName) continue;

                    // 计算在图片上的实际像素坐标
                    float x = label.Position.X * imgSize.Width;
                    float y = label.Position.Y * imgSize.Height;

                    // 颜色逻辑
                    bool isSelected = (label.Index == selectedIndex);
                    Brush themeBrush = isSelected ? Brushes.Purple : Brushes.Red;

                    // 1. 获取字号（如果没有设置或非法，给个保底值 12）
                    float currentFontSize = label.FontSize > 0 ? (float)label.FontSize : 12f;

                    // 2. 尝试创建用户指定的字体
                    Font textFont = null;
                    try
                    {
                        // 使用 label.FontFamily 创建字体
                        textFont = new Font(label.FontFamily, currentFontSize, FontStyle.Regular);
                    }
                    catch
                    {
                        // 容错：如果系统找不到该字体，回退到微软雅黑或默认字体
                        textFont = new Font("Microsoft YaHei", currentFontSize, FontStyle.Regular);
                    }

                    using (textFont)
                    using (Font indexFont = new Font("Arial", 12, FontStyle.Bold)) // 序号通常保持加粗好认
                    {
                        string idxStr = label.Index.ToString();

                        // 测量和计算对齐位置
                        SizeF size = g.MeasureString(idxStr, indexFont);
                        float centerX = x - (size.Width / 2f);
                        float centerY = y - (size.Height / 2f);

                        // A. 绘制序号
                        g.DrawString(idxStr, indexFont, themeBrush, centerX, centerY);

                        // B. 绘制内容（使用自定义字体）
                        if (!string.IsNullOrEmpty(label.Text))
                        {
                            // 间距随字号动态调整
                            float textX = centerX + (currentFontSize * 1.5f);
                            g.DrawString(label.Text, textFont, themeBrush, textX, y, vFormat);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 将特定格式的文本解析为 imageLabel 列表
        /// </summary>
        public static List<imageLabel> ParseTextToLabels(string content)
        {
            var result = new List<imageLabel>();
            // 兼容不同系统的换行符
            string[] lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            string currentImg = string.Empty;
            imageLabel currentLabel = null;

            // 正则匹配：图片名 >>>>>>>>[01.png]<<<<<<<<
            var imgRegex = new Regex(@">>>>>>>>\[(.*?)\]<<<<<<<<");
            // 正则匹配：元数据 ----------------[1]----------------[0.370,0.043,2]
            var metaRegex = new Regex(@"----------------\[(\d+)\]----------------\[([\d\.]+),([\d\.]+),(\d+)\]");

            foreach (string rawLine in lines)
            {
                string line = rawLine.Trim();
                if (string.IsNullOrEmpty(line)) continue;

                // 1. 识别图片行
                var imgMatch = imgRegex.Match(line);
                if (imgMatch.Success)
                {
                    currentImg = imgMatch.Groups[1].Value;
                    continue;
                }

                // 2. 识别元数据行
                var metaMatch = metaRegex.Match(line);
                if (metaMatch.Success)
                {
                    if (currentLabel != null) result.Add(currentLabel);

                    int index = int.Parse(metaMatch.Groups[1].Value);
                    float x = float.Parse(metaMatch.Groups[2].Value);
                    float y = float.Parse(metaMatch.Groups[3].Value);
                    int groupValue = int.Parse(metaMatch.Groups[4].Value);

                    currentLabel = new imageLabel
                    {
                        ImageName = currentImg,
                        Index = index,
                        Position = new BoundingBox { X = x, Y = y },
                        // 1 -> InsideBox, 2 -> OutsideBox
                        Group = groupValue == 1 ? LabelGroup.InsideBox : LabelGroup.OutsideBox,
                        Text = ""
                    };
                    continue;
                }

                // 3. 识别文本行（属于上一个元数据标签的内容）
                if (currentLabel != null && !line.StartsWith("-") && !line.StartsWith(">"))
                {
                    if (string.IsNullOrEmpty(currentLabel.Text))
                        currentLabel.Text = line;
                    else
                        currentLabel.Text += Environment.NewLine + line;
                }
            }

            if (currentLabel != null) result.Add(currentLabel);
            return result;
        }

        /// <summary>
        /// 将列表数据转换回特定的 txt 文本格式
        /// </summary>
        public static string LabelsToText(List<imageLabel> labels)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            // 1. 写入固定的头部信息（根据你提供的样本）
            sb.AppendLine("1,0");
            sb.AppendLine("-");
            sb.AppendLine("框内");
            sb.AppendLine("框外");
            sb.AppendLine("-");
            sb.AppendLine("Default Comment");
            sb.AppendLine("You can edit me");
            sb.AppendLine();
            sb.AppendLine();

            // 使用 Path.GetFileName 提取文件名
            var groupedLabels = labels.GroupBy(l => System.IO.Path.GetFileName(l.ImageName))
                                          .OrderBy(g => g.Key);

            foreach (var group in groupedLabels)
            {
                sb.AppendLine($">>>>>>>>[{group.Key}]<<<<<<<<");

                foreach (var label in group.OrderBy(l => l.Index))
                {
                    int groupValue = label.Group == LabelGroup.InsideBox ? 1 : 2;
                    sb.AppendLine($"----------------[{label.Index}]----------------[{label.Position.X:F3},{label.Position.Y:F3},{groupValue}]");
                    sb.AppendLine(label.Text);
                    sb.AppendLine();
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
