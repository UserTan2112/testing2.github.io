﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Activities.Statements;

public partial class EditProject : System.Web.UI.Page
{
    public static String CS = ConfigurationManager.ConnectionStrings["Project_A"].ConnectionString;
    string Material = "";
    string User = "";
    string Status = "";
    

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["LoginType"] == null || Session["LoginType"].ToString() != "Admin")
        {
            Response.Redirect("SignIn.aspx");
        }
        else
        {
            if (!IsPostBack)
            {
                BindGridview();

            }
        }
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        try
        {
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                DeleteQuantity();
                SqlCommand cmd = new SqlCommand("delete from tblProject where ProjectID=" + txtID.Text + "", con);
                cmd.ExecuteNonQuery();
                Response.Write("<script>alert('Delete successfully')</script>");
                BindGridview();
                clear();
            }
        }
        catch
        {
            Response.Write("<script>alert('" + "Something went wrong" + "')</script>");
        }
    }

    protected void txtID_TextChanged(object sender, EventArgs e)
    {
        try
        {
            using (SqlConnection con = new SqlConnection(CS))
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                SqlCommand cmd = new SqlCommand("select ProjectID,ProjectName,ProjectManager,ScenarioXiom,BillOfMaterial,Quantity,Status,UserID,Description from tblProject where ProjectID=@ProjectID", con);
                cmd.Parameters.AddWithValue("@ProjectID", Convert.ToInt32(txtID.Text));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                da.Fill(ds, "dt");
                con.Close();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    btnUpdate.Enabled = true;
                    btnDelete.Enabled = true;
                    txtUpdate.Text = ds.Tables[0].Rows[0]["ProjectName"].ToString();
                    txtPManager.Text = ds.Tables[0].Rows[0]["ProjectManager"].ToString();
                    txtScenarioXiom.Text = ds.Tables[0].Rows[0]["ScenarioXiom"].ToString();
                    txtDescription.Text = ds.Tables[0].Rows[0]["Description"].ToString();
                    Material = ds.Tables[0].Rows[0]["BillOfMaterial"].ToString();
                    txtQuantity.Text = ds.Tables[0].Rows[0]["Quantity"].ToString();
                    Status = ds.Tables[0].Rows[0]["Status"].ToString();
                    User = ds.Tables[0].Rows[0]["UserID"].ToString();
                    ddlStatus.SelectedValue = Status;
                    BindMaterial();
                    BindUsers();
                }
                else
                {
                    btnUpdate.Enabled = false;
                    btnDelete.Enabled = false;
                    txtUpdate.Text = string.Empty;
                }
                con.Close();
            }
        }
        catch (Exception ex)
        {
            Response.Write("<script>alert('" + "Please enter the ID First" + "')</script>");
        }
    }

    private void BindGridview()
    {
        using (SqlConnection con = new SqlConnection(CS))
        {
            if (con.State == ConnectionState.Closed) { con.Open(); }
            SqlDataAdapter da = new SqlDataAdapter("SELECT TOP (1000) p.[ProjectID]\r\n" +
            ",p.[ProjectName]\r\n" +
            ",p.[ProjectManager]\r\n" +
            ",p.[ScenarioXiom]\r\n" +
            ",p.[Description]\r\n" +
            ",i.[InventoryName] AS [BillOfMaterial]\r\n" +
            ",p.[Quantity]\r\n" +
            ",p.[Status]\r\n" +
            ",u.[Username]\r\n" +
            "FROM [Project_A].[dbo].[tblProject] p\r\n" +
            "JOIN [Project_A].[dbo].[tblInventory] i ON p.[BillOfMaterial] = i.[InventoryID]\r\n" +
            "JOIN [Project_A].[dbo].[tblUsers] u ON p.[UserID] = u.[UserID]", con); 
            DataTable dt = new DataTable();
                    da.Fill(dt); 
            con.Close();
            if (dt.Rows.Count > 0)
            {
                GridView1.DataSource = dt;
                GridView1.DataBind();
            }

            else
            {
                GridView1.DataSource = null;
                GridView1.DataBind();
            }
        }
    }

    private void BindMaterial()
    {
        using (SqlConnection con = new SqlConnection(CS))
        {
            if (con.State == ConnectionState.Closed) { con.Open(); }
            SqlCommand cmd = new SqlCommand("SELECT * from tblInventory", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            con.Close();
            if (dt.Rows.Count != 0)
            {
                ddlMaterials.DataSource = dt;
                ddlMaterials.DataTextField = "InventoryName";
                ddlMaterials.DataValueField = "InventoryID";
                ddlMaterials.DataBind();
                ddlMaterials.Items.Insert(0, new ListItem("-Select-", "0"));
                ddlMaterials.SelectedValue = Material;
            }
        }
    }

    private void BindUsers()
    {
        
            using (SqlConnection con = new SqlConnection(CS))
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                SqlCommand cmd = new SqlCommand("Select * from tblUsers", con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                con.Close();
                if (dt.Rows.Count != 0)
                {
                    ddlUser.DataSource = dt;
                    ddlUser.DataTextField = "Username";
                    ddlUser.DataValueField = "UserID";
                    ddlUser.DataBind();
                    ddlUser.Items.Insert(0, new ListItem("-Select-", "0"));
                    ddlUser.SelectedValue = User;
                }
            }
        
        
        
    }

    private void clear()
    {
        txtID.Text = string.Empty;
        txtUpdate.Text=string.Empty;
        txtPManager.Text = string.Empty;
        txtScenarioXiom.Text = string.Empty;
        txtQuantity.Text = string.Empty;
        ddlMaterials.SelectedIndex = 0;
        ddlStatus.SelectedIndex = 0;
        ddlUser.SelectedIndex = 0;
        txtminus.Text = string.Empty;
        txtplus.Text = string.Empty;
        txtDescription.Text = string.Empty;
    }


    protected void btnUpdate_Click1(object sender, EventArgs e)
    {
        if (ddlMaterials.SelectedIndex != 0 && ddlStatus.SelectedIndex != 0 && ddlUser.SelectedIndex != 0)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    if (con.State == ConnectionState.Closed) { con.Open(); }

                    SqlCommand cmd = new SqlCommand("update tblProject set ProjectName=@ProjectName, ProjectManager=@ProjectManager, ScenarioXiom=@ScenarioXiom, BillOfMaterial=@BillOfMaterial,Status=@Status, UserID=@UserID,Description=@Description where ProjectID=@ID", con);
                    cmd.Parameters.AddWithValue("@ID", Convert.ToInt32(txtID.Text));
                    cmd.Parameters.AddWithValue("@ProjectName", txtUpdate.Text);
                    cmd.Parameters.AddWithValue("@ProjectManager", txtPManager.Text);
                    cmd.Parameters.AddWithValue("@ScenarioXiom", txtScenarioXiom.Text);
                    cmd.Parameters.AddWithValue("@BillOfMaterial", ddlMaterials.SelectedValue);
                    cmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue);
                    cmd.Parameters.AddWithValue("@UserID", ddlUser.SelectedValue);
                    cmd.Parameters.AddWithValue("@Description", txtDescription.Text);
                    UpdateProject();
                    MinusProject();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    Response.Write("<script>alert('Update successfully')</script>");
                    clear();
                    BindGridview();

                }

            }

            catch (Exception ex)
            {
                Response.Write("<script>alert('" + "Something went Wrong" + "')</script>");
            }
        }
        else
        {
            Response.Write("<script>alert('Please fill up all the field')</script>");
        }
            
    }

    private void UpdateInventory()
    {
        using (SqlConnection con = new SqlConnection(CS))
        {
            if (con.State == ConnectionState.Closed) { con.Open(); }
            SqlCommand cmd = new SqlCommand("UPDATE tblInventory SET SellQuantity = SellQuantity + @Quantity WHERE InventoryID = @ID", con);
            cmd.Parameters.AddWithValue("@ID", ddlMaterials.SelectedValue);
            cmd.Parameters.AddWithValue("@Quantity", txtplus.Text);
            cmd.ExecuteNonQuery();
            con.Close();
            
        }
    }

    private void DeleteQuantity()
    {
        using (SqlConnection con = new SqlConnection(CS))
        {
            if (con.State == ConnectionState.Closed) { con.Open(); }
            SqlCommand cmd = new SqlCommand("UPDATE tblInventory SET SellQuantity = SellQuantity - @Quantity WHERE InventoryID = @ID", con);
            cmd.Parameters.AddWithValue("@ID", ddlMaterials.SelectedValue);
            cmd.Parameters.AddWithValue("@Quantity", txtQuantity.Text);
            cmd.ExecuteNonQuery();
            con.Close();

        }
    }

    private void MinusQuantity()
    {
        using (SqlConnection con = new SqlConnection(CS))
        {

            if (con.State == ConnectionState.Closed) { con.Open(); }
            SqlCommand cmd = new SqlCommand("UPDATE tblInventory SET SellQuantity = SellQuantity - @Quantity WHERE InventoryID = @ID", con);
            cmd.Parameters.AddWithValue("@ID", ddlMaterials.SelectedValue);
            cmd.Parameters.AddWithValue("@Quantity", txtminus.Text);
            cmd.ExecuteNonQuery();
            con.Close();

        }
    }
    private void UpdateProject()
    {
        using (SqlConnection con = new SqlConnection(CS))
        {
            if (con.State == ConnectionState.Closed) { con.Open(); }
            SqlCommand cmd = new SqlCommand("UPDATE tblProject SET Quantity = Quantity + @Quantity WHERE ProjectID = @ID", con);
            cmd.Parameters.AddWithValue("@ID", txtID.Text);
            cmd.Parameters.AddWithValue("@Quantity", txtplus.Text);
            cmd.ExecuteNonQuery();
            UpdateInventory();
            con.Close();

        }
    }

    private void MinusProject()
    {
        using (SqlConnection con = new SqlConnection(CS))
        {
            if (con.State == ConnectionState.Closed) { con.Open(); }
            SqlCommand cmd = new SqlCommand("UPDATE tblProject SET Quantity = Quantity - @Quantity WHERE ProjectID = @ID", con);
            cmd.Parameters.AddWithValue("@ID", txtID.Text);
            cmd.Parameters.AddWithValue("@Quantity", txtminus.Text);
            MinusQuantity();
            cmd.ExecuteNonQuery();
            con.Close();

        }
    }

    protected void btnSelect_Click(object sender, EventArgs e)
    {
        int rowId = ((GridViewRow)(sender as Control).NamingContainer).RowIndex;
        GridViewRow selectedRow = GridView1.Rows[rowId];
        string inventoryID = selectedRow.Cells[1].Text;
        txtID.Text = inventoryID;

        // Trigger the txtID_TextChanged event
        txtID_TextChanged(sender, e);
    }
}