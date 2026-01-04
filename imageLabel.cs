using System;

public class imageLabel
{
    public imageLabel()
    {
        FontSize = 12.0;
        FontFamily = "Microsoft YaHei";
        Group = LabelGroup.InsideBox;
        Remark = "You can edit me";
        Position = BoundingBox.Default;
    }
    /// <summary>
    /// 标注所属的图片名
    /// </summary>
    public string ImageName { get; set; } = string.Empty;

    /// <summary>
    /// 标注在图片中的序号
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// 标注在图片中的相对位置
    /// 以左上角为原点，范围 [0,1]
    /// </summary>
    public BoundingBox Position { get; set; }

    /// <summary>
    /// 标注内容（文字）
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// 字号（支持一位小数）
    /// </summary>
    public double FontSize { get; set; }

    /// <summary>
    /// 字体名称
    /// </summary>
    public string FontFamily { get; set; } = string.Empty;

    /// <summary>
    /// 所属分组（如：框内 / 框外）
    /// </summary>
    public LabelGroup Group { get; set; }


    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; } = string.Empty;


}

public struct BoundingBox
{
    // 左上角 x
    public float X { get; set; }

    // 左上角 y
    public float Y { get; set; }

    // 宽度
    public float Width { get; set; }

    // 高度
    public float Height { get; set; }

    public static BoundingBox Default => new BoundingBox
    {
        X = 0f,
        Y = 0f,
        Width = 0f,
        Height = 0f
    };
}

public enum LabelGroup
{
    InsideBox,
    OutsideBox,
    Other
}
