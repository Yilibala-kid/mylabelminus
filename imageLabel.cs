using System.ComponentModel;
using System.Runtime.CompilerServices;


public class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value)) return false;
        storage = value;
        OnPropertyChanged(propertyName);
        return true;
    }
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}// 基础通知类，减少重复代码
public class ImageInfo : ViewModelBase
{
    private string _imageName = string.Empty;
    public string ImageName { get => _imageName; set => SetProperty(ref _imageName, value); }
    public BindingList<ImageLabel> Labels { get; set; } = new BindingList<ImageLabel>();// 该图片下的所有标注（子节点）

    private bool _isRefreshing = false; // 锁：防止递归
    public ImageInfo()
    {
        // 自动管理索引：当列表发生增删、重排时自动更新 Index
        Labels.ListChanged += (s, e) =>
        {
            // 关键点 1：如果是 Reset 类型，通常说明是我们自己刚刷新完，直接跳过
            if (_isRefreshing || e.ListChangedType == ListChangedType.Reset) return;

            // 关键点 2：仅对增、删、移动进行序号刷新
            if (e.ListChangedType == ListChangedType.ItemAdded ||
                e.ListChangedType == ListChangedType.ItemDeleted ||
                e.ListChangedType == ListChangedType.ItemMoved)
            {
                RefreshIndices();
            }
        };
    }
    public void RefreshIndices()
    {
        if (_isRefreshing) return;
        _isRefreshing = true;

        // 暂时关闭事件通知
        var oldRaiseEvents = Labels.RaiseListChangedEvents;
        Labels.RaiseListChangedEvents = false;

        try
        {
            for (int i = 0; i < Labels.Count; i++)
            {
                // 只有当序号真的变了才赋值，减少 PropertyChanged 触发
                if (Labels[i].Index != i + 1)
                    Labels[i].Index = i + 1;
            }
        }
        finally
        {
            Labels.RaiseListChangedEvents = oldRaiseEvents;
            _isRefreshing = false;

            // 只有在确定需要 UI 刷新时才调用
            if (oldRaiseEvents)
            {
                Labels.ResetBindings();
            }
        }
    }

    public void AddLabel(ImageLabel label)
    {
        // 因为你已经在构造函数里写了 Labels.ListChanged 监听
        // 所以这里只需要简单 Add，RefreshIndices 会自动被触发
        Labels.Add(label);
    }
    public ImageLabel AddLabelFromPixels(float x, float y, float w, float h, int imgW, int imgH)
    {
        var label = new ImageLabel
        {
            Position = new BoundingBox(x / imgW, y / imgH, w / imgW, h / imgH)
        };
        Labels.Add(label);
        return label;
    }// 快捷添加方法：直接传入像素坐标和图像尺寸，内部自动转为归一化比例
}
public class ImageLabel : ViewModelBase
{
    private int _index;
    private string _text = "";
    private double _fontSize = 12.0;
    private string _fontFamily = "微软雅黑";
    private string _group = "框内";
    private string _remark = "这是备注";
    private BoundingBox _position = BoundingBox.Default;

    [DisplayName("序号")]
    [ReadOnly(true)]
    public int Index { get => _index; set => SetProperty(ref _index, value); }

    [DisplayName("文本内容")]
    public string Text { get => _text; set => SetProperty(ref _text, value); }

    [DisplayName("分组")]
    public string Group { get => _group; set => SetProperty(ref _group, value); }

    [DisplayName("位置")]
    public BoundingBox Position
    {
        get => _position;
        set => SetProperty(ref _position, value); // 仅执行基础赋值
    }
    [Browsable(false)]
    public float X
    {
        get => _position.X;
        set
        {
            if (_position.X == value) return;
            _position = _position with { X = value };
            OnPropertyChanged(); // 仅通知 X 改变
            OnPropertyChanged(nameof(Position)); // 通知整体改变
        }
    }

    [Browsable(false)]
    public float Y
    {
        get => _position.Y;
        set
        {
            if (_position.Y == value) return;
            _position = _position with { Y = value };
            OnPropertyChanged();
            OnPropertyChanged(nameof(Position));
        }
    }
    [Browsable(false)]
    public float Width
    {
        get => _position.Width;
        set
        {
            if (_position.Width == value) return;
            _position = _position with { Width = value };
            OnPropertyChanged();
            OnPropertyChanged(nameof(Position));
        }
    }
    [Browsable(false)] 
    public float Height
    {
        get => _position.Height;
        set
        {
            if (_position.Height == value) return;
            _position = _position with { Height = value };
            OnPropertyChanged();
            OnPropertyChanged(nameof(Position));
        }
    }
    [DisplayName("字号")] public double FontSize { get => _fontSize; set => SetProperty(ref _fontSize, value); }
    [DisplayName("字体")] public string FontFamily { get => _fontFamily; set => SetProperty(ref _fontFamily, value); }
    [DisplayName("备注")] public string Remark { get => _remark; set => SetProperty(ref _remark, value); }
    public ImageLabel Clone() => (ImageLabel)this.MemberwiseClone();

}

public struct BoundingBox
{
    public float X { get; init; }
    public float Y { get; init; }
    public float Width { get; init; }
    public float Height { get; init; }

    public BoundingBox(float x, float y, float w, float h) { X = x; Y = y; Width = w; Height = h; }
    public static BoundingBox Default => new BoundingBox(0, 0, 0, 0);

    // 重写 ToString 方便在 PropertyGrid 或 DataGridView 中预览
    public override string ToString() => $"X: {X:F3}\nY: {Y:F3}";
}



