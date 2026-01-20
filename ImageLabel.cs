using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;


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
    private bool _isRefreshing = false; // 锁：防止递归
    public BindingList<ImageLabel> Labels { get; set; } = [];// 该图片下的所有标注（子节点）

    public string ImageName { get => _imageName; set => SetProperty(ref _imageName, value); }
    [Browsable(false)] public List<ImageLabel> ActiveLabels => Labels.Where(l => !l.IsDeleted).ToList();
    

    public ImageInfo()
        {
            Labels.ListChanged += (s, e) => {
                if (!_isRefreshing && e.ListChangedType is ListChangedType.ItemAdded or ListChangedType.ItemDeleted or ListChangedType.ItemMoved)
                    RefreshIndices();
            };
        }

    public void RefreshIndices()
    {
        _isRefreshing = true;
        Labels.RaiseListChangedEvents = false;

        int nextIndex = 1;

        // 1. 先给没删除的标签分配连续序号
        var activeLabels = Labels.Where(l => !l.IsDeleted).OrderBy(l => l.Index).ToList();
        foreach (var lbl in activeLabels)
        {
            lbl.Index = nextIndex++;
        }

        // 2. 给已删除的标签分配一个较大的序号（可选，方便导出时排序）
        var deletedLabels = Labels.Where(l => l.IsDeleted);
        foreach (var lbl in deletedLabels)
        {
            // 可以设为 999 或者保持原样，但建议递增以防导出 Diff 时乱序
            lbl.Index = nextIndex++;
        }

        Labels.RaiseListChangedEvents = true;
        Labels.ResetBindings();
        _isRefreshing = false;
    }
    public void ResetModificationFlags()
    {
        foreach (var l in Labels) l.IsModified = false;
    }

}
public class ImageLabel : ViewModelBase
{
    #region 基本属性
    private int _index;
    private string _text = "";
    private string _originalText = ""; // 存储原文
    private double _fontSize = 16.0;
    private string _fontFamily = "微软雅黑";
    private string _group = "框内";
    private string _remark = "这是备注";
    private BoundingBox _position = BoundingBox.Default;
    private bool _isModified = false;
    private bool _isDeleted = false;

    public void LoadBaseContent(string text)// 初始化时调用，设定原文且不触发 Modified
    {
        _originalText = text;
        _text = text;
        _isModified = false;
        OnPropertyChanged(nameof(Text));
    }
    [DisplayName("序号")] public int Index { get => _index; set => SetProperty(ref _index, value); }

    
    [DisplayName("文本内容")] public string Text
    {
        get => _text;
        set
        {
            if (SetProperty(ref _text, value))
            {
                IsModified = true; // 仅在这里标记
            }
        }
    }
    [DisplayName("分组")]public string Group { get => _group; set => SetProperty(ref _group, value); }
    [DisplayName("位置")] public BoundingBox Position { get => _position; set => SetProperty(ref _position, value); }
    private void UpdatePos(Func<BoundingBox, BoundingBox> updater, [CallerMemberName] string prop = "")
    {
        var newVal = updater(_position);
        if (EqualityComparer<BoundingBox>.Default.Equals(_position, newVal)) return;
        _position = newVal;
        OnPropertyChanged(prop);
        OnPropertyChanged(nameof(Position));
    }
    [Browsable(false)] public float X { get => _position.X; set => UpdatePos(p => p with { X = Math.Clamp(value, 0, 1) }); }
    [Browsable(false)] public float Y { get => _position.Y; set => UpdatePos(p => p with { Y = Math.Clamp(value, 0, 1) }); }
    [Browsable(false)] public float Width { get => _position.Width; set => UpdatePos(p => p with { Width = value }); }
    [Browsable(false)] public float Height { get => _position.Height; set => UpdatePos(p => p with { Height = value }); }

    [DisplayName("字号")] public double FontSize { get => _fontSize; set => SetProperty(ref _fontSize, value); }
    [DisplayName("字体")] public string FontFamily { get => _fontFamily; set => SetProperty(ref _fontFamily, value); }
    [DisplayName("备注")] public string Remark { get => _remark; set => SetProperty(ref _remark, value); }

    #endregion

    #region 修改label
    [Browsable(false)]
    public bool IsDeleted
    {
        get => _isDeleted;
        set => SetProperty(ref _isDeleted, value);
    }

    // 修改 IsModified 的逻辑：如果被标记删除，也属于已修改
    [Browsable(false)]
    public bool IsModified
    {
        get => _isModified || _isDeleted;
        set => SetProperty(ref _isModified, value);
    }
    public string OriginalText => _originalText;

    // 封装一个方法：如果是用户手动修改，则标记为已修改
    protected new bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
    {
        bool changed = base.SetProperty(ref storage, value, propertyName);
        // 关键逻辑：除了 Index 变化外，其他属性变化都视为“已修改”
        if (changed && propertyName != nameof(Index) && propertyName != nameof(IsModified))
        {
            IsModified = true;
        }
        return changed;
    }
    #endregion
    public ImageLabel Clone() => (ImageLabel)this.MemberwiseClone();

}

public readonly record struct BoundingBox(float X, float Y, float Width, float Height)
{
    public static BoundingBox Default => new(0, 0, 0, 0);
}



