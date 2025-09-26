using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace BindingListTest;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.DataContext = new MainViewModel();
    }
}

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    public partial ObservableBindingList<Item> Items { get; set; } = [];

    [ObservableProperty]
    public partial string Name { get; set; } = "Item 1";

    [ObservableProperty]
    public partial int Value { get; set; } = 5;

    public int TotalValue => Items.Sum(i => i.Value);

    public MainViewModel()
    {
        Items.ListChanged += (s, e) => OnPropertyChanged(nameof(TotalValue));
    }

    [RelayCommand]
    public void Add()
    {
        Items.Add(new Item { Name = this.Name, Value = this.Value });
        Name = "Item " + (Items.Count+1);
    }

    [RelayCommand]
    public void Remove(Item item)
    {
        Items.Remove(item);
    }
}
public partial class Item : ObservableObject
{
    [ObservableProperty]
    public partial string? Name { get; set; }

    [ObservableProperty]
    public partial int Value { get; set; }
}

[ObservableObject]
public partial class ObservableBindingList<T> : BindingList<T>, INotifyCollectionChanged
{
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    protected override void InsertItem(int index, T item)
    {
        base.InsertItem(index, item);
        CollectionChanged?.Invoke(index, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        OnPropertyChanged(nameof(Count));
    }

    protected override void RemoveItem(int index)
    {
        var item = this[index];
        base.RemoveItem(index);
        CollectionChanged?.Invoke(index, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
        OnPropertyChanged(nameof(Count));
    }
}