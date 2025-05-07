using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MidtermsPresentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        DataClasses1DataContext db = new DataClasses1DataContext(Properties.Settings.Default.MidtermsConnectionString);
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LogInBtn_Click(object sender, RoutedEventArgs e)
        {
            if (userTb.Text == "" || passwordTb.Text == "")
            {
                MessageBox.Show("Please enter both username and password.");
                return;
            }

            Class1 user = getUserCredentials();
            if (user == null)
            {
                return;
            }

            if (passcompare(user.UserPassWord) == 0)
            {
                MessageBox.Show("Login Success");

                // Check the role and take action accordingly
                if (user.UserRole == "admin")
                {
                    AdminHome adminHome = new AdminHome();
                    adminHome.Show();
                    this.Close();
                }
                else if (user.UserRole == "librarian")
                {
                    MessageBox.Show("librarian logic here");
                }
                else if (user.UserRole == "student")
                {
                    MessageBox.Show("student logic here");
                }

                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid password");
            }
        }
        private Class1 getUserCredentials()
        {
            var user = (from s in db.UserTables
                        where s.UserName == userTb.Text
                        select new Class1 { UserPassWord = s.UserPassWord, UserRole = s.UserRole }).FirstOrDefault();

            if (user == null)
            {
                MessageBox.Show("User not found");
                return null;
            }

            return user;
        }
        private int passcompare(string sPass)
        {
            if (passwordTb.Text == sPass)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
    }
}
