using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LvItemTest
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		TestVM vm;
		public MainWindow()
		{
			InitializeComponent();

			vm = new TestVM();

			this.DataContext = vm;

			vm.LoadCollection();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{

		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			vm.ReLoadCollection();
		}
	}
	public class BaseViewModel : INotifyPropertyChanged
	{

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public class TestVM : BaseViewModel
	{
		System.Timers.Timer _timer;
		System.Timers.Timer _timerReloadTest;
		public ICommand AddCommand { get; }
		public ICommand RemoveCommand { get; }
		public ICommand RemoveAllCommand { get; }
		private string _selectedString;
		public string SelectedString
		{
			get
			{
				return _selectedString;
			}
			set
			{
				_selectedString = value;
				OnPropertyChanged();
			}
		}
		public TestVM()
		{
			AddCommand = new RelayCommand(AddCommandFunc);
			RemoveCommand = new RelayCommand(RemoveCommandFunc);
			RemoveAllCommand = new RelayCommand(RemoveAllCommandFunc);
		}

		private async void AddCommandFunc(object obj)
		{
			if (TestBindCollectionWeak !=null)
			{
				var random = new Random();
				TestBindCollectionWeak.Add("String " + random.Next(1000).ToString());
				random = null;
			}
		}

		private async void RemoveCommandFunc(object obj) 
		{
			if (SelectedString !=null)
			{
				if (TestBindCollectionWeak.Contains(SelectedString))
				{
					TestBindCollectionWeak.Remove(SelectedString);
				}
			}
		}

		private async void RemoveAllCommandFunc(object obj)
		{
			TestBindCollectionWeak.Clear();
		}

		private void OnTimerReloadElapsed(object sender, ElapsedEventArgs e)
		{
		}

		public async void LoadCollection()
		{
			var random = new Random();
			for (int i = 0; i < 100000; i++)
			{
				string randomString = "String " + random.Next(1000).ToString();
				TestBindCollectionWeak.Add(randomString);
			}
		}

		public async void ReLoadCollection()
		{
			TestBindCollectionWeak.Clear();
			var random = new Random();
			for (int i = 0; i < 100000; i++)
			{
				string randomString = "String " + random.Next(1000).ToString();
				TestBindCollectionWeak.Add(randomString);
			}
		}

		private WeakObservableCollection<string> _testBindCollectionWeak = new WeakObservableCollection<string>();

		public WeakObservableCollection<string> TestBindCollectionWeak
		{
			get
			{
				return _testBindCollectionWeak;
			}
			set
			{
				_testBindCollectionWeak = value;
				OnPropertyChanged();
			}
		}

		private ObservableCollection<string> _testColl = new ObservableCollection<string>();

		private ObservableCollection<string> TestColl
		{
			get
			{
				return _testColl;
			}
			set
			{
				_testColl = value;
				OnPropertyChanged();
			}
		}

		private WeakReference _selectedTest;

		public WeakReference SelectedTest
		{
			get
			{
				return _selectedTest;
			}
			set
			{
				_selectedTest = value;
				OnPropertyChanged();
			}
		}

	}



	public class RelayCommand : ICommand
	{
		private Action<object> execute;
		private Func<object, bool> canExecute;

		public RelayCommand()
		{
		}

		public RelayCommand(Action<object> executeAction, Func<object, bool>? canExecuteFunc = null)
		{
			execute = executeAction;
			canExecute = canExecuteFunc;
		}

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		public bool CanExecute(object parameter)
		{
			return canExecute == null || canExecute(parameter);
		}

		public void Execute(object parameter)
		{
			execute(parameter);
		}
	}

	public class WeakList<T> : IEnumerable<T> where T : class
	{
		private readonly List<WeakReference<T>> _list = new();
		private int _operations = 0;

		public int Count => _list.Count;

		public int CleanupThreshold { get; set; } = 8;

		public void Add(T item)
		{
			if (item == null) throw new ArgumentNullException(nameof(item));
			CleanupIfNeeded();
			_list.Add(new WeakReference<T>(item));
		}

		public bool Remove(T item)
		{
			if (item == null) throw new ArgumentNullException(nameof(item));

			bool removed = false;

			_list.RemoveAll(w =>
			{
				if (w.TryGetTarget(out T? target) && target == item)
				{
					removed = true;
					return true;
				}
				return !w.TryGetTarget(out _); 
			});

			return removed;
		}

		public void Clear()
		{
			_list.Clear();
			_operations = 0;
		}

		public IEnumerator<T> GetEnumerator()
		{
			CleanupIfNeeded();
			foreach (var w in _list)
			{
				if (w.TryGetTarget(out T? item))
					yield return item;
			}
		}


		private void CleanupIfNeeded()
		{
			if (++_operations > CleanupThreshold)
			{
				_operations = 0;
				_list.RemoveAll(w => !w.TryGetTarget(out _));
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}

	public class WeakObservableCollection<T> : ObservableCollection<T> where T : class
	{
		private readonly WeakList<T> _weakList = new();

		public WeakObservableCollection() { }

		public WeakObservableCollection(IEnumerable<T> collection) : base(collection)
		{
			foreach (var item in collection)
				_weakList.Add(item);
		}

		public new void Add(T item)
		{
			if (item == null) throw new ArgumentNullException(nameof(item));
			_weakList.Add(item);
			base.Add(item);
		}

		public new bool Remove(T item)
		{
			if (item == null) throw new ArgumentNullException(nameof(item));
			bool removedFromWeakList = _weakList.Remove(item);
			bool removedFromBase = base.Remove(item);
			return removedFromWeakList && removedFromBase; 
		}

		public new void Clear()
		{
			_weakList.Clear();
			base.Clear(); 
		}

		public IEnumerable<T> GetActiveItems()
		{
			return _weakList;
		}
	}









}