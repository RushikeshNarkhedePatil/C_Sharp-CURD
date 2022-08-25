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
            using (MySqlConnection mysqlCon=new MySqlConnection(conectionString))
            {
                mysqlCon.Open();
                MySqlCommand mysqlCmd = new MySqlCommand("BookAddOrEdit", mysqlCon);
                mysqlCmd.CommandType = CommandType.StoredProcedure;
                mysqlCmd.Parameters.AddWithValue("_BookID", bookID);
                mysqlCmd.Parameters.AddWithValue("_BookName", txtName.Text.Trim());
                mysqlCmd.Parameters.AddWithValue("_Author", txtAuthor.Text.Trim());
                mysqlCmd.Parameters.AddWithValue("_Description", txtDescription.Text.Trim());
                mysqlCmd.ExecuteNonQuery();
                MessageBox.Show("Submited sucessfully");
                Clear();
                GridFill();
            }
        }
        void GridFill()
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

        private void Form1_Load(object sender, EventArgs e)
        {
            Clear();
            GridFill();
        }
        void Clear()
        {
            txtName.Text = txtAuthor.Text = txtDescription.Text = txtSearch.Text = "";
            //txtName.Clear();
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
    }
}
