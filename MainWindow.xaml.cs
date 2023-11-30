using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using static EntityFramworkWpfApp.Animal;


namespace EntityFramworkWpfApp
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public class Animal
    {
        public int Id {  get; set; }
        public string Name { set; get; }
        public Animal() { }
        public Animal(string name)
        {
            Name = name;
        }

        public void Print()
        {
            MessageBox.Show($"Id: {Id}, Name: {Name}");
        }


        public class AnimalContext : DbContext
        {
            public string connectionString = @"Data Source=HomeDE\SQLEXPRESS;Initial Catalog=AnimalDB;Integrated Security=True;Encrypt=False";
            public DbSet<Animal> Animal { set; get; }
            public AnimalContext() {
                //Database.EnsureDeleted();
                Database.EnsureCreated();
            }
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { optionsBuilder.UseSqlServer(connectionString); }
        }


    }
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<Animal> _animals;

        public ObservableCollection<Animal> Animals
        {
            get { return _animals; }
            set
            {
                _animals = value;
                OnPropertyChanged(nameof(Animals));
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                using (AnimalContext db = new AnimalContext())
                {
                    Animal animal1 = new Animal("Cat");
                    Animal animal2 = new Animal("Dog");
                    db.Animal.Add(animal1);
                    db.Animal.Add(animal2);
                    db.SaveChanges();
                    var animals = db.Animal.ToList();
                    foreach (var animal in animals)
                    {
                        animal.Print();
                    }
                }
            }
            catch (SqlException ex) { MessageBox.Show(ex.Message); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            //--

            try
            {
                using (AnimalContext db = new AnimalContext())
                {
                    Animals = new ObservableCollection<Animal>(db.Animal.ToList());
                    DG_Table.ItemsSource = Animals;
                }
            }
            catch (SqlException ex) { MessageBox.Show(ex.Message); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}