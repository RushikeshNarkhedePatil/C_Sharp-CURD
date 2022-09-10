using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.Windows.Input;
namespace LocalDB
{
    public partial class Form1 : Form
    {
        string conectionString = @"SERVER=127.0.0.1;Database=bookdb;Uid=root;Pwd=toor;";
        int bookID=0;
       
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            try
            {
                using (MySqlConnection mysqlCon = new MySqlConnection(conectionString))
                {
                    mysqlCon.Open();

                    MySqlCommand mysqlCmd = new MySqlCommand("BookAddOrEdit", mysqlCon);
                    mysqlCmd.CommandType = CommandType.StoredProcedure;
                    mysqlCmd.Parameters.AddWithValue("_BookID", bookID);
                    mysqlCmd.Parameters.AddWithValue("_BookName", txtName.Text.Trim());
                    mysqlCmd.Parameters.AddWithValue("_Author", txtAuthor.Text.Trim());
                    mysqlCmd.Parameters.AddWithValue("_Description", txtDescription.Text.Trim());
                    if (txtName.Text == "" || txtAuthor.Text == "" || txtDescription.Text == "")
                    {
                        MessageBox.Show("Please fill in all fields", "Error");
                        txtName.Focus(); // set focus to lastNameTextBox
                    }
                    else if (!Regex.Match(txtName.Text, @"^([a-zA-Z]+|[a-zA-Z]+\s[a-zA-Z]+)$").Success)
                    {
                        MessageBox.Show("Invalid Book Name", "Book Name", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtName.Focus();
                    }
                    else if (!Regex.Match(txtAuthor.Text, @"^([a-zA-Z]+|[a-zA-Z]+\s[a-zA-Z]+)$").Success)
                    {
                        MessageBox.Show("Invalid Author Name", "Author Name", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtAuthor.Focus();
                    }
                    else if (!Regex.Match(txtDescription.Text, @"^([a-zA-Z]+|[a-zA-Z]+\s[a-zA-Z]+)$").Success)
                    {
                        MessageBox.Show("Invalid Description", "Description", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtDescription.Focus();
                    }
                    // if(name==true&&Author==true&&Desc==true)
                    else
                    {
                        mysqlCmd.ExecuteNonQuery();
                        MessageBox.Show("Submited sucessfully", "Sucess");
                        logger.Info("Data store in database sucessfully");
                        Clear();
                        GridFill();
                    }

                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message,"Error");
                logger.Error(ex.Message);
            }
            
        }
        void GridFill()
        {
            try
            {
                using (MySqlConnection mysqlCon = new MySqlConnection(conectionString))
                {
                    mysqlCon.Open();
                    MySqlDataAdapter sqlDa = new MySqlDataAdapter("BookViewAll", mysqlCon);
                    sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                    DataTable dtblBook = new DataTable();
                    sqlDa.Fill(dtblBook);
                    dgvBook.DataSource = dtblBook;
                    dgvBook.Columns[0].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"message");
             

            }
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           // this.WindowState = FormWindowState.Maximized;
            try
            {
                Clear();
                GridFill();
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show("m form loading error" + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("m form loading error" + ex.Message);
            }
           
        }
        void Clear()
        {
            txtName.Text = txtAuthor.Text = txtDescription.Text = txtSearch.Text = "";
            bookID = 0;
            btnSave.Text = "Save";
            btnDelete.Enabled = false;
        }

        private void dgvBook_DoubleClick(object sender, EventArgs e)
        {
            if(dgvBook.CurrentRow.Index != -1)
            {
                txtName.Text = dgvBook.CurrentRow.Cells[1].Value.ToString();
                txtAuthor.Text = dgvBook.CurrentRow.Cells[2].Value.ToString();
                txtDescription.Text = dgvBook.CurrentRow.Cells[3].Value.ToString();
                bookID = Convert.ToInt32(dgvBook.CurrentRow.Cells[0].Value.ToString());
                btnSave.Text = "Update";
                btnDelete.Enabled = Enabled;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            using (MySqlConnection mysqlCon = new MySqlConnection(conectionString))
            {
                mysqlCon.Open();
                MySqlDataAdapter sqlDa = new MySqlDataAdapter("BookSearchByValue", mysqlCon);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.SelectCommand.Parameters.AddWithValue("_SearchValue", txtSearch.Text);
                DataTable dtblBook = new DataTable();
                sqlDa.Fill(dtblBook);
                dgvBook.DataSource = dtblBook;
                dgvBook.Columns[0].Visible = false;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            using (MySqlConnection mysqlCon = new MySqlConnection(conectionString))
            {
                mysqlCon.Open();
                MySqlCommand mysqlCmd = new MySqlCommand("BookDeleteByID", mysqlCon);
                mysqlCmd.CommandType = CommandType.StoredProcedure;
                mysqlCmd.Parameters.AddWithValue("_BookID", bookID);
                mysqlCmd.ExecuteNonQuery();
                MessageBox.Show("Deleted sucessfully");
                Clear();
                GridFill();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
           // this.CancelButton = myCancelBtn;
           System.Windows.Forms.Application.Exit();
        }

        private void hedding_Click(object sender, EventArgs e)
        {

        }

        private void txtName_Validating(object sender, CancelEventArgs e)
        {
            if(string.IsNullOrEmpty(txtName.Text))
            {
                e.Cancel = true;
                txtName.Focus();
                errorProvider1.SetError(txtName, "please enter book name !");
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtName, null);
            }
        }

        private void txtAuthor_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtAuthor.Text))
            {
                e.Cancel = true;
                txtAuthor.Focus();
                errorProvider2.SetError(txtAuthor, "please enter book name !");
            }
            else
            {
                e.Cancel = false;
                errorProvider2.SetError(txtAuthor, null);
            }
        }

        private void txtDescription_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtDescription.Text))
            {
                e.Cancel = true;
                txtDescription.Focus();
                errorProvider2.SetError(txtDescription, "please enter book name !");
            }
            else
            {
                e.Cancel = false;
                errorProvider3.SetError(txtDescription, null);
            }
        }

        //export report 
        private void btnExport_Click(object sender, EventArgs e)
        {
            if (dgvBook.Rows.Count > 0)
            {
                Microsoft.Office.Interop.Excel.Application xcelApp = new Microsoft.Office.Interop.Excel.Application();
                xcelApp.Application.Workbooks.Add(Type.Missing);

                for (int i = 1; i < dgvBook.Columns.Count + 1; i++)
                {
                    xcelApp.Cells[1, i] = dgvBook.Columns[i - 1].HeaderText;
                }
                for (int i = 0; i < dgvBook.Rows.Count; i++)
                {
                    for (int j = 0; j < dgvBook.Columns.Count; j++)
                    {
                        xcelApp.Cells[i + 2, j + 1] = dgvBook.Rows[i].Cells[j].Value.ToString();
                    }
                }
                xcelApp.Columns.AutoFit();
                xcelApp.Visible = true;
            }
        }
    }
}
