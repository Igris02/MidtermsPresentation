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
using System.Windows.Shapes;

namespace MidtermsPresentation
{
    /// <summary>
    /// Interaction logic for AdminManager.xaml
    /// </summary>
    public partial class AdminManager : Window
    {
        DataClasses1DataContext db = new DataClasses1DataContext(Properties.Settings.Default.MidtermsConnectionString);

        private UserTable SelectedUser;
        public AdminManager()
        {
            InitializeComponent();
            LoadUsers();
            LoadRoles();
        }
        private void LoadUsers()
        {
            try
            {
                // 4. Clear the DataGrid before loading new data
                //CRUD Operation: Read
                var users = from u in db.UserTables select u;
                UserDataGrid.ItemsSource = users.ToList();

                Status.Text = "Users loaded successfully.";
            }
            catch (Exception ex)
            {
                // 5. Handle any exceptions that occur during the loading of users
                MessageBox.Show("Error loading users: " + ex.Message);
                Status.Text = "Error loading users.";
            }
        }

        private void Adduser_Click(object sender, RoutedEventArgs e)
        {
            int userId = int.Parse(adduserID.Text); // if you're now using numeric IDs
            string password = adduserpassword.Text.Trim(); 
            string userName = addusername.Text.Trim();
            string selectedRole = addroleCB.SelectedItem?.ToString();


            if (!string.IsNullOrEmpty(userId.ToString()) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(selectedRole))
            {
                // 10. Check if the user ID already exists in the database
                
                if (!db.UserTables.Any(u => u.UserID == userId))
                {
                    // 11. Create a new user object and set its properties
                    var newUser = new UserTable
                    {
                        // 12. Set the properties of the new user object
                        UserName = userName,
                        UserPassWord = password,
                        UserRole = selectedRole
                    };

                    // 13. Insert the new user object into the database using InsertOnSubmit method
                    db.UserTables.InsertOnSubmit(newUser);
                    db.SubmitChanges();

                    try
                    {
                        // 14. Submit the changes to the database using SubmitChanges method
                        db.SubmitChanges();

                        // 15. Reload the users to reflect the changes in the DataGrid
                        LoadUsers();

                        // 16. Clear the text boxes and combo box after successful addition
                        Status.Text = $"User '{userId}' added successfully with Role: '{selectedRole}'.";
                        adduserID.Clear();
                        adduserpassword.Clear();
                        addusername.Clear();
                        addroleCB.SelectedIndex = -1; // Clear selection
                    }
                    catch (Exception ex)
                    {
                        // 17. Handle any exceptions that occur during the addition of the user
                        MessageBox.Show("Error adding user: " + ex.Message);
                        Status.Text = "Error adding user.";
                    }
                }
                else
                {
                    // 18. Show a message if the user ID already exists
                    MessageBox.Show($"User with ID '{userId}' already exists.");
                    Status.Text = "User already exists.";
                }
            }
            else
            {
                // 19. Show a message if any of the fields are empty 
                MessageBox.Show("User ID, password, user name, and role cannot be empty.");
                Status.Text = "User ID, password, user name, and role cannot be empty.";
            }
        }

        private void UserDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UserDataGrid.SelectedItem != null && UserDataGrid.SelectedItem is UserTable selectedUser)
            {
                // 22. Set the selected user to the _selectedUser variable
                SelectedUser = selectedUser;

                updateUserIdTextBox.Text = SelectedUser.UserID.ToString();
                updatePasswordTextBox.Text = SelectedUser.UserPassWord;
                updateUserNameTextBox.Text = SelectedUser.UserName;
                updateRoleComboBox.SelectedItem = SelectedUser.UserRole; // make sure roles are strings, not ComboBoxItems

                viewUserIdTextBox.Text = SelectedUser.UserID.ToString();
                viewPasswordTextBox.Text = SelectedUser.UserPassWord;
                viewUserNameTextBox.Text = SelectedUser.UserName;
                viewRoleTextBox.Text = SelectedUser.UserRole;

                // 25. Enable the save and delete buttons
                UpdateBtn.IsEnabled = true;
                DeleteBtn.IsEnabled = true;
            }

            // 26. If no item is selected, clear the text boxes and disable the buttons
            else
            {
                selectedUser = null;
                updateUserIdTextBox.Clear();
                updatePasswordTextBox.Clear();
                updateUserNameTextBox.Clear();
                updateRoleComboBox.SelectedIndex = -1;
                viewUserIdTextBox.Clear();
                viewPasswordTextBox.Clear();
                viewUserNameTextBox.Clear();
                viewRoleTextBox.Clear();
                UpdateBtn.IsEnabled = false;
                DeleteBtn.IsEnabled = false;
            }
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedUser != null)
            {
                // 29. Get the values from the text boxes and combo box
                SelectedUser.UserPassWord = updatePasswordTextBox.Text.Trim();
                SelectedUser.UserName = updateUserNameTextBox.Text.Trim();
                string selectedRole = updateRoleComboBox.SelectedItem?.ToString();

                // 30. Check if the selected role is not empty
                if (!string.IsNullOrEmpty(selectedRole))
                {
                    // 31. Update the selected user's properties
                    SelectedUser.UserRole = selectedRole; // Update the UserRole

                    try
                    {
                        // 32. Submit the changes to the database using SubmitChanges method
                        db.SubmitChanges();

                        // 33. Reload the users to reflect the changes in the DataGrid
                        LoadUsers();

                        // 34. Clear the text boxes and combo box after successful update
                        Status.Text = $"User '{SelectedUser.UserID}' updated successfully with Role: '{selectedRole}'.";
                        updateUserIdTextBox.Clear();
                        updatePasswordTextBox.Clear();
                        updateUserNameTextBox.Clear();
                        updateRoleComboBox.SelectedIndex = -1;
                        viewUserIdTextBox.Clear();
                        viewPasswordTextBox.Clear();
                        viewUserNameTextBox.Clear();
                        viewRoleTextBox.Clear();
                        UpdateBtn.IsEnabled = false;
                        SelectedUser = null; // Clear the selected user
                    }
                    catch (Exception ex)
                    {
                        // 35. Handle any exceptions that occur during the update of the user
                        MessageBox.Show("Error updating user: " + ex.Message);
                        Status.Text = "Error updating user.";
                    }
                }
                else
                {
                    // 36. Show a message if the selected role is empty
                    MessageBox.Show("Please select a role to update.");
                    Status.Text = "Please select a role to update.";
                }
            }
            else
            {
                // 37. Show a message if no user is selected for update
                MessageBox.Show("No user selected for update.");
                Status.Text = "No user selected for update.";
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (UserDataGrid.SelectedItem != null && UserDataGrid.SelectedItem is UserTable selectedUser)
            {
                // 39. Set the selected user to the _selectedUser variable
                SelectedUser = selectedUser;

                // 41. Confirm the deletion with the user
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete user '{selectedUser.UserID}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                // 42. If the user confirms, delete the selected user
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // 43. Delete the selected user from the database using DeleteOnSubmit method then submit the changes using SubmitChanges method
                        string deletedUserID = SelectedUser.UserID.ToString(); // Store the UserID before setting _selectedUser to null

                        db.UserTables.DeleteOnSubmit(SelectedUser);
                        db.SubmitChanges();

                        //44. Recreate the DataContext to clear its cache
                        db = new DataClasses1DataContext(Properties.Settings.Default.MidtermsConnectionString);

                        // 45. Reload the users to reflect the changes in the DataGrid
                        LoadUsers();

                        // 46. Clear the text boxes and combo box after successful deletion
                        Status.Text = $"User '{deletedUserID}' deleted successfully."; // Use the stored UserID
                        DeleteBtn.IsEnabled = false;
                        SelectedUser = null;
                        updateUserIdTextBox.Clear();
                        updatePasswordTextBox.Clear();
                        updateUserNameTextBox.Clear();
                        updateRoleComboBox.SelectedIndex = -1;
                        viewUserIdTextBox.Clear();
                        viewPasswordTextBox.Clear();
                        viewUserNameTextBox.Clear();
                        viewRoleTextBox.Clear();
                        UpdateBtn.IsEnabled = false;
                    }
                    catch (Exception ex)
                    {
                        // 47. Handle any exceptions that occur during the deletion of the user 
                        MessageBox.Show($"Error deleting user: {ex.Message}", "Deletion Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        Status.Text = "Error deleting user.";

                    }
                }
            }
            else
            {
                // 48. Show a message if no user is selected for deletion
                MessageBox.Show("No user selected for deletion.");
                Status.Text = "No user selected for deletion.";
            }
        }
        private void LoadRoles()
        {
            var roles = new List<string> { "student", "admin", "librarian" };

            addroleCB.ItemsSource = roles;
            updateRoleComboBox.ItemsSource = roles;
        }
        private void ReturnBtn1_Click(object sender, RoutedEventArgs e)
        {
            AdminHome adminHOme = new AdminHome();
            adminHOme.Show();
            this.Close();
        }

        private void LGBtn1_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
