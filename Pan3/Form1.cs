﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Entidades;
using Negocios;
using Microsoft.VisualBasic;
using System.Drawing.Printing;



namespace Pan3
{
    public partial class Form1 : Form
    {

        #region variables globales

        private string idproveedor;
        private string idcliente;
        private string idautorizado;
        private string idproducto;
        private string idcategoria;
        private bool editarse = false;
        decimal Pagado = 0;
        private int contador = 0;
        private bool EncontreDuplicado = false;

        decimal preciototal = 0;
        decimal preciototalC = 0;

        public string NombreAutorizado = string.Empty;
        public int IdAutorizado = 0;
        public decimal MontoInicialCaja = 0;

        #endregion 

        #region Entidades y objetos

        E_Proveedor objEProveedor = new E_Proveedor();
        NegProveedores objNegProveedor = new NegProveedores();
        E_Cliente objECliente = new E_Cliente();
        NegCliente objNegCliente = new NegCliente();
        E_Autorizados objEAutorizado = new E_Autorizados();
        NegAutorizados objNegAutorizado = new NegAutorizados();
        E_Categoria objECategoria = new E_Categoria();
        NegCategoria objNegCategoria = new NegCategoria();
        E_Producto objEProducto = new E_Producto();
        NegProducto objNegProducto = new NegProducto();
        NegVenta objNegVenta = new NegVenta();
        E_Ventas objEVenta = new E_Ventas();
        E_ProductoVenta objEProductoVenta = new E_ProductoVenta();
        NegProductoVenta objNegProductoVenta = new NegProductoVenta();
        E_FormaDePago objEFormaDePago = new E_FormaDePago();
        NegFormaDePago objNegFormaDePago = new NegFormaDePago();
        E_Deuda objEDeuda = new E_Deuda();
        NegDeuda objNegDeuda = new NegDeuda();
        E_Compra objECompra = new E_Compra();
        NegCompra objNegCompra = new NegCompra();
        E_ProductoCompra objEProductoCompra = new E_ProductoCompra();
        NegProductoCompra objNegProductoCompra = new NegProductoCompra();
        E_Caja objECaja = new E_Caja();
        NegCaja objNegCaja = new NegCaja();

        #endregion
        public Form1()
        {
            InitializeComponent();

            CrearColumnas();
            CrearColumnasAut();
            CrearColumnasProd();
            CrearColumnasCat();
            CrearColumnasPago();
            CrearColumnasCaja();
            CrearColumnasEgresosCaja();
            CrearColumnasProveedores();
            CrearColumnasCompra();

            lbltime.Text = DateTime.Now.ToString();
            BTVenta.BackgroundImageLayout = ImageLayout.Stretch;
            BTRemover.BackgroundImageLayout = ImageLayout.Stretch;
            BTCancelar.BackgroundImageLayout = ImageLayout.Stretch;
            CBProducto.AutoCompleteMode = AutoCompleteMode.Suggest;

            #region ToolTip

            ToolTip TTventa = new ToolTip();
            TTventa.ShowAlways = true;
            TTventa.SetToolTip(BTVenta, "Finalizar Venta.\r\nSuma la totalidad de los productos y efectúa la venta.");
            TTventa.IsBalloon = true;
            TTventa.AutoPopDelay = 15000;

            ToolTip TTremover = new ToolTip();
            TTremover.ShowAlways = true;
            TTremover.SetToolTip(BTRemover, "Remover Producto.\r\nRemueve todas las unidades del producto seleccionado de la lista de venta.");
            TTremover.IsBalloon = true;
            TTremover.AutoPopDelay = 15000;

            ToolTip TTcancelar = new ToolTip();
            TTcancelar.ShowAlways = true;
            TTcancelar.SetToolTip(BTCancelar, "Cancelar Venta.\r\nRemueve todos los productos de la lista de venta.");
            TTcancelar.IsBalloon = true;
            TTcancelar.AutoPopDelay = 15000;

            #endregion
        }
        private void TimerHora_Tick(object sender, EventArgs e)
        {
            lbltime.Text = DateTime.Now.ToString("HH:mm\r\ndd/MM/yyyy");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            accionestabla();
            LlenarDGV();
            LlenarDGVAut();
            LlenarDGVCat();
            LlenarDGVProd();
            LlenarDGVCaja();
            LlenarCajaEgresos();
            LlenarCbCat();
            LlenarCbProductos();
            MostarAut();
            LlenarCbClientes();
            FormaDePago();
            LlenarTablaProveedores();
            LlenarCbProveedores();
            LlenarCbProd();
            FormaDePagoC();

            DataSet ds = objNegCaja.datosUltimoAutorizadoConCajaAbierta();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                MontoInicialCaja = decimal.Parse(dr[2].ToString());
            }

            txtMontoInicial.Text = MontoInicialCaja.ToString();
        }

        #region Proveedor

        public void accionestabla()
        {
            dgvProv.Columns[0].Visible = false;
            dgvProv.Columns[8].Visible = false;
        }

        private void LimpiarTxt()
        {
            txtprov.Text = string.Empty;
            txtrs.Text = string.Empty;
            txtmail.Text = string.Empty;
            txttel.Text = string.Empty;
            txttelR.Text = string.Empty;
            txtdom.Text = string.Empty;
            txtcuil.Text = string.Empty;

            txtprov.Focus();
        }

        private void CrearColumnasProveedores()
        {
            dgvProv.ColumnCount = 9;
            dgvProv.Columns[0].HeaderText = "Id";
            dgvProv.Columns[1].HeaderText = "Nombre proveedor";
            dgvProv.Columns[2].HeaderText = "Razon Social";
            dgvProv.Columns[3].HeaderText = "E-mail";
            dgvProv.Columns[4].HeaderText = "Tel proveedor";
            dgvProv.Columns[5].HeaderText = "Tel repartidor";
            dgvProv.Columns[6].HeaderText = "Domicilio";
            dgvProv.Columns[7].HeaderText = "CUIL";
            dgvProv.Columns[8].HeaderText = "Estado";
        }

        private void LlenarTablaProveedores()
        {
            dgvProv.Rows.Clear();
            DataSet ds = new DataSet();
            ds = objNegProveedor.ListandoProveedor("Todos");
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    dgvProv.Rows.Add(dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), dr[6].ToString(), dr[7].ToString(), dr[8].ToString());
                }
            }
            else
                MessageBox.Show("No hay clientes cargados en el sistema");
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvProv.SelectedRows.Count > 0)
            {
                editarse = true;
                idproveedor = dgvProv.CurrentRow.Cells[0].Value.ToString();
                txtprov.Text = dgvProv.CurrentRow.Cells[1].Value.ToString();
                txtrs.Text = dgvProv.CurrentRow.Cells[2].Value.ToString();
                txtmail.Text = dgvProv.CurrentRow.Cells[3].Value.ToString();
                txttel.Text = dgvProv.CurrentRow.Cells[4].Value.ToString();
                txttelR.Text = dgvProv.CurrentRow.Cells[5].Value.ToString();
                txtdom.Text = dgvProv.CurrentRow.Cells[6].Value.ToString();
                txtcuil.Text = dgvProv.CurrentRow.Cells[7].Value.ToString();
            }
            else if (dgvProv.SelectedRows.Count <= 0)
            {
                MessageBox.Show("Seleccione el proveedor que desea editar");
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (editarse == false)
            {
                try
                {
                    objEProveedor.Name = txtprov.Text;
                    objEProveedor.Razonsocial = txtrs.Text;
                    objEProveedor.Mail = txtmail.Text;
                    objEProveedor.Tel = txttel.Text;
                    objEProveedor.Telrep = txttelR.Text;
                    objEProveedor.Dom = txtdom.Text;
                    objEProveedor.Cuil = txtcuil.Text;

                    objNegProveedor.InsertandoPrroveedor("Alta", objEProveedor);

                    MessageBox.Show("Proveedor guardado");
                    //mostrarBuscarTablaP(string.Empty);
                    LlenarTablaProveedores();
                    LimpiarTxt();
                }
                catch (Exception ex)
                {

                    MessageBox.Show("No se pudo guardar el proveedor" + ex);
                }
            }

            if (editarse == true)
            {
                try
                {
                    objEProveedor.Id = Convert.ToInt32(idproveedor);
                    objEProveedor.Name = txtprov.Text;
                    objEProveedor.Razonsocial = txtrs.Text;
                    objEProveedor.Mail = txtmail.Text;
                    objEProveedor.Tel = txttel.Text;
                    objEProveedor.Telrep = txttelR.Text;
                    objEProveedor.Dom = txtdom.Text;
                    objEProveedor.Cuil = txtcuil.Text;

                    objNegProveedor.EditandoProveedor("Modificar", objEProveedor);

                    MessageBox.Show("Proveedor editado");
                    //mostrarBuscarTablaP(string.Empty);
                    LlenarTablaProveedores();
                    editarse = false;
                    LimpiarTxt();

                }
                catch (Exception ex)
                {

                    MessageBox.Show("No se pudo editar el proveedor" + ex);
                }
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvProv.SelectedRows.Count > 0)
            {
                objEProveedor.Id = Convert.ToInt32(idproveedor);
                objEProveedor.Estado = true;
                objNegProveedor.EliminandoProveedor("Eliminar", objEProveedor);

                MessageBox.Show("Se eliminó el proveedor correctamente");
                // mostrarBuscarTablaP(string.Empty);
                LlenarTablaProveedores();
            }
            else
            {
                MessageBox.Show("Seleccione el proveedor que desea eliminar");
            }
        }

        #endregion

        #region Cliente

        private void CrearColumnas()
        {
            dgvCliente.ColumnCount = 7;
            dgvCliente.Columns[0].HeaderText = "Id";
            dgvCliente.Columns[1].HeaderText = "Nombre cliente";
            dgvCliente.Columns[2].HeaderText = "Nombre negocio";
            dgvCliente.Columns[3].HeaderText = "Domicilio";
            dgvCliente.Columns[4].HeaderText = "E-mail";
            dgvCliente.Columns[5].HeaderText = "Telefono";
            dgvCliente.Columns[6].HeaderText = "Estado";
        }
        private void LlenarDGV()
        {
            dgvCliente.Rows.Clear();
            DataSet ds = new DataSet();
            ds = objNegCliente.ListandoClientes("Todos");
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    dgvCliente.Rows.Add(dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), dr[6].ToString());
                }
            }
        }

        private void BtnG_Click(object sender, EventArgs e)
        {
            if (editarse == false)
            {
                try
                {
                    objECliente.Nombre_cl = txtNomCliente.Text;
                    objECliente.Nombre_neg = TxtNomCom.Text;
                    objECliente.Dom = TxtDomCliente.Text;
                    objECliente.Mail = TxtEmailCliente.Text;
                    objECliente.Tel = TxtTelCliente.Text;
                    objECliente.Esta_cancelado = false;

                    objNegCliente.InsertandoCliente("Alta", objECliente);
                    MessageBox.Show("Cliente guardado");
                    LlenarDGV();
                    LimpiarTxtC();
                }
                catch (Exception ex)
                {

                    MessageBox.Show("No se pudo guardar el cliente" + ex);
                }
            }

            if (editarse == true)
            {
                try
                {
                    objECliente.Id = Convert.ToInt32(idcliente);
                    objECliente.Nombre_cl = txtNomCliente.Text;
                    objECliente.Nombre_neg = TxtNomCom.Text;
                    objECliente.Dom = TxtDomCliente.Text;
                    objECliente.Mail = TxtEmailCliente.Text;
                    objECliente.Tel = TxtTelCliente.Text;

                    objNegCliente.EditandoCliente("Modificar", objECliente);

                    MessageBox.Show("Cliente editado");
                    LimpiarTxt();
                    LlenarDGV();
                }
                catch (Exception ex)
                {

                    MessageBox.Show("No se pudo editar el cliente" + ex);
                }
            }

            LlenarCbClientes();
        }

        private void LimpiarTxtC()
        {
            txtNomCliente.Text = string.Empty;
            TxtNomCom.Text = string.Empty;
            TxtDomCliente.Text = string.Empty;
            TxtEmailCliente.Text = string.Empty;
            TxtTelCliente.Text = string.Empty;

            txtNomCliente.Focus();
        }

        private void BtnEditC_Click(object sender, EventArgs e)

        {
            editarse = true;

            if (dgvCliente.SelectedRows.Count > 0)
            {
                idcliente = dgvCliente.CurrentRow.Cells[0].Value.ToString();
                txtNomCliente.Text = dgvCliente.CurrentRow.Cells[1].Value.ToString();
                TxtNomCom.Text = dgvCliente.CurrentRow.Cells[2].Value.ToString();
                TxtDomCliente.Text = dgvCliente.CurrentRow.Cells[3].Value.ToString();
                TxtEmailCliente.Text = dgvCliente.CurrentRow.Cells[4].Value.ToString();
                TxtTelCliente.Text = dgvCliente.CurrentRow.Cells[5].Value.ToString();

            }
            else if (dgvCliente.SelectedRows.Count <= 0)
            {
                MessageBox.Show("Seleccione el proveedor que desea editar");
            }

            LlenarCbClientes();
        }

        private void btncancelarC_Click(object sender, EventArgs e)
        {
            LimpiarTxtC();
        }

        private void BtnEC_Click(object sender, EventArgs e)
        {
            if (dgvCliente.SelectedRows.Count > 0)
            {
                objECliente.Id = Convert.ToInt32(dgvCliente.CurrentRow.Cells[0].Value.ToString());
                objECliente.Esta_cancelado = true;
                objNegCliente.EliminandoCliente("Eliminar", objECliente);

                MessageBox.Show("Se eliminó el cliente correctamente");
                LlenarDGV();
            }
            else
            {
                MessageBox.Show("Seleccione el cliente que desea eliminar");
            }

            LlenarCbClientes();
        }

        #endregion

        #region Autorizado

        private void CrearColumnasAut()
        {
            dgvAutorizados.ColumnCount = 6;
            dgvAutorizados.Columns[0].HeaderText = "Id";
            dgvAutorizados.Columns[1].HeaderText = "Nombre";
            dgvAutorizados.Columns[2].HeaderText = "Apellido";
            dgvAutorizados.Columns[3].HeaderText = "Usuario";
            dgvAutorizados.Columns[4].HeaderText = "Clave";
            dgvAutorizados.Columns[5].HeaderText = "Estado";

            dgvAutorizados.Columns[0].Visible = false;
        }

        private void MostarAut()
        {
            lblAut.Text = lblAut.Text + NombreAutorizado;
        }
        private void LlenarDGVAut()
        {
            dgvAutorizados.Rows.Clear();
            DataSet ds = new DataSet();
            ds = objNegAutorizado.ListandoAutorizados("Todos");
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    dgvAutorizados.Rows.Add(dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString());
                }
            }
        }

        private void BtnGAut_Click(object sender, EventArgs e)
        {
            if (editarse == false)
            {
                try
                {
                    objEAutorizado.Nombre_aut = txtNomAut.Text;
                    objEAutorizado.Apellido_aut = txtApAut.Text;
                    objEAutorizado.Usuario_aut = txtUsAut.Text;
                    objEAutorizado.Clave_aut = txtClaveAut.Text;
                    objEAutorizado.Esta_cancelado = false;

                    objNegAutorizado.InsertandoAutorizados("Alta", objEAutorizado);
                    MessageBox.Show("Autorizado guardado");
                    LlenarDGVAut();
                    LimpiarTxtA();
                }
                catch (Exception ex)
                {

                    MessageBox.Show("No se pudo guardar el autorizado" + ex);
                }
            }

            if (editarse == true)
            {
                try
                {
                    objEAutorizado.Id = Convert.ToInt32(idautorizado);
                    objEAutorizado.Nombre_aut = txtNomAut.Text;
                    objEAutorizado.Apellido_aut = txtApAut.Text;
                    objEAutorizado.Usuario_aut = txtUsAut.Text;
                    objEAutorizado.Clave_aut = txtClaveAut.Text;

                    objNegAutorizado.EditandoAutorizados("Modificar", objEAutorizado);

                    MessageBox.Show("Autorizado editado");
                    LimpiarTxtA();
                    LlenarDGVAut();
                }
                catch (Exception ex)
                {

                    MessageBox.Show("No se pudo editar el autorizado" + ex);
                }
            }
        }
        private void LimpiarTxtA()
        {
            txtNomAut.Text = string.Empty;
            txtApAut.Text = string.Empty;
            txtUsAut.Text = string.Empty;
            txtClaveAut.Text = string.Empty;
            txtConfCAut.Text = string.Empty;

            txtNomAut.Focus();
        }

        private void BtnEdAut_Click(object sender, EventArgs e)
        {
            editarse = true;

            if (dgvAutorizados.SelectedRows.Count > 0)
            {
                idautorizado = dgvAutorizados.CurrentRow.Cells[0].Value.ToString();
                txtNomAut.Text = dgvAutorizados.CurrentRow.Cells[1].Value.ToString();
                txtApAut.Text = dgvAutorizados.CurrentRow.Cells[2].Value.ToString();
                txtUsAut.Text = dgvAutorizados.CurrentRow.Cells[3].Value.ToString();
                txtClaveAut.Text = dgvAutorizados.CurrentRow.Cells[4].Value.ToString();
            }
            else if (dgvAutorizados.SelectedRows.Count <= 0)
            {
                MessageBox.Show("Seleccione el autorizado que desea editar");
            }
        }

        private void BtnElAut_Click(object sender, EventArgs e)
        {
            if (dgvAutorizados.SelectedRows.Count > 0)
            {
                objEAutorizado.Id = Convert.ToInt32(dgvAutorizados.CurrentRow.Cells[0].Value.ToString());
                objEAutorizado.Esta_cancelado = true;
                objNegAutorizado.EliminandoAutorizados("Eliminar", objEAutorizado);

                MessageBox.Show("Se eliminó el autorizado correctamente");
                LlenarDGV();
            }
            else
            {
                MessageBox.Show("Seleccione el autorizado que desea eliminar");
            }
        }
        #endregion

        #region Producto

        private void CrearColumnasProd()
        {
            dgvProductos.ColumnCount = 7;
            dgvProductos.Columns[0].HeaderText = "Id";
            dgvProductos.Columns[1].HeaderText = "Codigo";
            dgvProductos.Columns[2].HeaderText = "Nombre";
            dgvProductos.Columns[3].HeaderText = "Stock";
            dgvProductos.Columns[4].HeaderText = "Precio de compra";
            dgvProductos.Columns[5].HeaderText = "Precio de venta";
            dgvProductos.Columns[6].HeaderText = "Categoría";

            dgvProductos.Columns[0].Visible = false;

        }

        private void LlenarDGVProd()
        {
            dgvProductos.Rows.Clear();
            DataSet ds = new DataSet();
            ds = objNegProducto.ListandoProductos("Todos");
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    dgvProductos.Rows.Add(dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), dr[6].ToString());
                }
            }
        }

        private void LimpiarTxtProd()
        {
            txtCodProd.Text = string.Empty;
            txtNomProd.Text = string.Empty;
            txtStockP.Text = string.Empty;
            txtPrecioCompra.Text = string.Empty;
            txtPrecioVenta.Text = string.Empty;
        }

        private void btnGProd_Click(object sender, EventArgs e)
        {
            if (editarse == false)
            {
                try
                {
                    objEProducto.Cod_prod = Convert.ToInt32(txtCodProd.Text);
                    objEProducto.Nombre_prod = txtNomProd.Text;
                    objEProducto.Stock_prod = Convert.ToInt32(txtStockP.Text);
                    objEProducto.P_compra = Convert.ToDecimal(txtPrecioCompra.Text);
                    objEProducto.P_venta = Convert.ToDecimal(txtPrecioVenta.Text);
                    objEProducto.Idcat = Convert.ToInt32(cbCategoria.SelectedIndex);

                    objNegProducto.InsertandoProducto("Alta", objEProducto);
                    MessageBox.Show("Producto guardado");
                    LlenarDGVProd();
                    LimpiarTxtProd();
                }
                catch (Exception ex)
                {

                    MessageBox.Show("No se pudo guardar el producto" + ex);
                }
            }

            if (editarse == true)
            {
                try
                {
                    objEProducto.Id = Convert.ToInt32(idproducto);
                    objEProducto.Cod_prod = Convert.ToInt32(txtCodProd.Text);
                    objEProducto.Nombre_prod = txtNomProd.Text;
                    objEProducto.Stock_prod = Convert.ToInt32(txtStockP.Text);
                    objEProducto.P_compra = Convert.ToDecimal(txtPrecioCompra.Text);
                    objEProducto.P_venta = Convert.ToDecimal(txtPrecioVenta.Text);
                    objEProducto.Idcat = Convert.ToInt32(cbCategoria.SelectedIndex);


                    objNegProducto.EditandoProducto("Modificar", objEProducto);

                    MessageBox.Show("Producto editado");
                    LimpiarTxtProd();
                    LlenarDGVProd();
                }
                catch (Exception ex)
                {

                    MessageBox.Show("No se pudo editar el producto" + ex);
                }
            }

            LlenarCbProductos();
        }

        private void btnEdProd_Click(object sender, EventArgs e)
        {
            editarse = true;

            if (dgvProductos.SelectedRows.Count > 0)
            {
                idproducto = dgvProductos.CurrentRow.Cells[0].Value.ToString();
                txtCodProd.Text = dgvProductos.CurrentRow.Cells[1].Value.ToString();
                txtNomProd.Text = dgvProductos.CurrentRow.Cells[2].Value.ToString();
                txtStockP.Text = dgvProductos.CurrentRow.Cells[3].Value.ToString();
                txtPrecioCompra.Text = dgvProductos.CurrentRow.Cells[4].Value.ToString();
                txtPrecioVenta.Text = dgvProductos.CurrentRow.Cells[5].Value.ToString();
                cbCategoria.Text = dgvProductos.CurrentRow.Cells[6].Value.ToString();

            }
            else if (dgvProductos.SelectedRows.Count <= 0)
            {
                MessageBox.Show("Seleccione el producto que desea editar");
            }

            LlenarCbProductos();
        }

        private void btnElProd_Click(object sender, EventArgs e)
        {
            if (dgvProductos.SelectedRows.Count > 0)
            {
                objEProducto.Id = Convert.ToInt32(dgvProductos.CurrentRow.Cells[0].Value.ToString());
                objNegProducto.EditandoProducto("Eliminar", objEProducto);

                MessageBox.Show("Se eliminó el producto correctamente");
                LlenarDGVProd();
            }
            else
            {
                MessageBox.Show("Seleccione el producto que desea eliminar");
            }

            LlenarCbProductos();
        }

        #endregion

        #region Categorias

        private void CrearColumnasCat()
        {
            dgvCategorias.ColumnCount = 3;
            dgvCategorias.Columns[0].HeaderText = "Id";
            dgvCategorias.Columns[1].HeaderText = "Nombre";
            dgvCategorias.Columns[2].HeaderText = "Código";

            dgvCategorias.Columns[0].Visible = false;
        }

        private void LimpiarTxtCat()
        {
            txtCodCat.Text = string.Empty;
            txtNomCat.Text = string.Empty;

            txtCodCat.Focus();
        }

        private void LlenarDGVCat()
        {
            dgvCategorias.Rows.Clear();
            DataSet ds = new DataSet();
            ds = objNegCategoria.ListandoCategorias("Todos");
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    dgvCategorias.Rows.Add(dr[0].ToString(), dr[1].ToString(), dr[2].ToString());
                }
            }
            else
                MessageBox.Show("No hay categorias cargadas en el sistema");
        }

        private void LlenarCbCat()

        {
            cbCategoria.Items.Clear();
            Datos.DatosConexionDB datosConexionDB = new Datos.DatosConexionDB();
            datosConexionDB.AbrirConexion();
            SqlCommand cmd = new SqlCommand("select * from categoria", datosConexionDB.Conexion);
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                cbCategoria.Items.Add(dr[1].ToString());
                cbCategoria.ValueMember = dr[0].ToString(); ;
            }
            datosConexionDB.CerrarConexion();
            cbCategoria.Items.Insert(0, string.Empty);
            cbCategoria.SelectedIndex = 0;
        }

        private void btnGCat_Click(object sender, EventArgs e)
        {
            if (editarse == false)
            {
                try
                {
                    objECategoria.Cod = txtCodCat.Text;
                    objECategoria.Name = txtNomCat.Text;

                    objNegCategoria.InsertandoCategoria("Alta", objECategoria);
                    MessageBox.Show("Categoría guardada");
                    LlenarDGVCat();
                    LimpiarTxtCat();
                }
                catch (Exception ex)
                {

                    MessageBox.Show("No se pudo guardar la categoría" + ex);
                }
            }

            if (editarse == true)
            {
                try
                {
                    objECategoria.Id = Convert.ToInt32(idcategoria);
                    objECategoria.Cod = txtCodCat.Text;
                    objECategoria.Name = txtNomCat.Text;

                    objNegCategoria.EditandoCategoria("Modificar", objECategoria);

                    MessageBox.Show("Categoría editada");
                    LimpiarTxtCat();
                    LlenarDGVCat();
                }
                catch (Exception ex)
                {

                    MessageBox.Show("No se pudo editar la categoria" + ex);
                }
            }
        }

        private void btnEdCat_Click(object sender, EventArgs e)
        {
            editarse = true;

            if (dgvCategorias.SelectedRows.Count > 0)
            {
                idcategoria = dgvCategorias.CurrentRow.Cells[0].Value.ToString();
                txtCodCat.Text = dgvAutorizados.CurrentRow.Cells[1].Value.ToString();
                txtNomCat.Text = dgvAutorizados.CurrentRow.Cells[2].Value.ToString();
            }
            else
            {
                MessageBox.Show("Seleccione la categoría que desea editar");
            }
        }

        private void btnElCat_Click(object sender, EventArgs e)
        {
            if (dgvCategorias.SelectedRows.Count > 0)
            {
                objECategoria.Id = Convert.ToInt32(dgvCategorias.CurrentRow.Cells[0].Value.ToString());
                objNegCategoria.EliminandoCategoria("Eliminar", objECategoria);

                MessageBox.Show("Se eliminó la categoría correctamente");
                LlenarDGVCat();
            }
            else
            {
                MessageBox.Show("Seleccione la categoría que desea eliminar");
            }
        }

        #endregion

        #region Venta

        private void BTVenta_Click(object sender, EventArgs e)
        {
            GuardarVenta();
            GuardarDeuda();
            MostrarDeuda();
            LlenarDGVCaja();
            LimpiarCampos();
            contador = 0;
            EncontreDuplicado = false;
        }

        private void GuardarVenta()
        {
            try
            {
                objEProductoVenta.Id_producto = Convert.ToInt32(CBProducto.SelectedValue.ToString());
                if (objEProductoVenta.Id_producto == 0)
                {
                    ProdPan();
                    GuardarProductoVenta();
                }

                objEProductoVenta.Cantidad = Convert.ToInt32(TxtCantidad.Text);
                objEVenta.Id_cliente1 = Convert.ToInt32(CBCliente.SelectedValue.ToString());
                objEVenta.Id_autorizado1 = IdAutorizado;
                objEVenta.Id_fpago1 = Convert.ToInt32(CbFPago.SelectedValue.ToString());
                objEVenta.Monto1 = Convert.ToDecimal(txtPagado.Text);
                objEVenta.Fecha_compra1 = DateTime.Now.ToString("d");
                objEVenta.Hora_venta1 = DateTime.Now.ToString("hh:mm tt"); ;
                objEVenta.Estado_trans1 = "cancelada";
                objEVenta.Num_Factura1 = string.Empty;

                objNegVenta.InsertandoVenta("Alta", objEVenta);
                GuardarProductoVenta();
                objNegProducto.ActualizarStock("RestaStock", objEProductoVenta);

                MessageBox.Show("Venta realizada con éxito");

                DGVListaVenta.Rows.Clear();
                DGVmdp.Rows.Clear();
                lbltotal.Text = string.Empty;
                TBStock.Text = string.Empty;
                TBPrecioU.Text = string.Empty;
                TBCode.Text = string.Empty;
                TBCategoría.Text = string.Empty;
                TxtCantidad.Text = string.Empty;

                LlenarDGVProd();
            }
            catch (Exception e)
            {
                MessageBox.Show("La venta no se pudo realizar" + e.Message);
            }
        }
        private void BTAgregar_Click(object sender, EventArgs e)
        {
            if (camposVaciosProducto())
            {
                if (contador < 1)
                {
                    CargarGrid();
                }
                else
                {
                    BuscarDuplicado();

                    if (!EncontreDuplicado)
                    {
                        CargarGrid();
                    }
                }
                CalcularTotalVenta();
            }
        }

        private void CargarGrid()
        {
            decimal precioxcantidad = Convert.ToInt32(TxtCantidad.Text) * Convert.ToInt32(TBPrecioU.Text);
            DGVListaVenta.Rows.Add(Convert.ToInt32(CBProducto.SelectedValue.ToString()), TxtCantidad.Text, CBProducto.Text, TBPrecioU.Text, precioxcantidad);
            preciototal = 0;
            contador++;
        }

        private void BuscarDuplicado() 
        {
            for (int i = 0; i < DGVListaVenta.Rows.Count; i++)
            {
                if ((DGVListaVenta.Rows[i].Cells[2].Value).ToString() == CBProducto.Text)
                {
                    int sumaCantidad = Convert.ToInt32(DGVListaVenta.Rows[i].Cells[1].Value);
                    DGVListaVenta.Rows[i].Cells[1].Value = (sumaCantidad + Convert.ToInt32(TxtCantidad.Text)).ToString();
                    DGVListaVenta.Rows[i].Cells[4].Value =
                        (Convert.ToInt32(DGVListaVenta.Rows[i].Cells[1].Value) *
                         Convert.ToInt32(DGVListaVenta.Rows[i].Cells[3].Value)).ToString();
                    EncontreDuplicado = true;
                    break;
                }
                else
                {
                    EncontreDuplicado = false;
                }
            }
        }

        private void CalcularTotalVenta()
        {
            for (int i = 0; i < DGVListaVenta.Rows.Count; ++i)
            {
                preciototal += Convert.ToDecimal(DGVListaVenta.Rows[i].Cells[4].Value);
            }
            lbltotal.Text = preciototal.ToString();
            txtADeuda.Text = lbltotal.Text;
        }

        private void LlenarCbProductos()
        {
            DataSet ds = new DataSet();
            ds = objNegProducto.ListandoProductos("Todos");
            CBProducto.DisplayMember = "Nombre_producto";
            CBProducto.ValueMember = "Id_producto";
            CBProducto.DataSource = ds.Tables[0];
        }

        private void CBProducto_SelectedIndexChanged(object sender, EventArgs e)
        {
            CBProducto.SelectedValue.ToString();

            DataSet ds = new DataSet();
            ds = objNegProducto.ListandoProductos(CBProducto.SelectedValue.ToString());
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    CBProducto.SelectedValue.ToString();
                    TBStock.Text = (dr["Stock_producto"].ToString());
                    TBPrecioU.Text = (dr["Preciouv_producto"].ToString());
                    TBCode.Text = (dr["Cod_producto"].ToString());
                    TBCategoría.Text = (dr["Id_categoria"].ToString());
                }
            }
        }


        private void LlenarCbClientes()
        {
            DataSet ds = new DataSet();
            ds = objNegCliente.ListandoClientes("Todos");
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    CBCliente.DisplayMember = "nombre_cliente";
                    CBCliente.ValueMember = "id_cliente";
                    CBCliente.DataSource = ds.Tables[0];
                    CBCliente.SelectedIndex = 0;
                }
            }
        }

        private void CBCliente_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblDeuda.Text = "0";
            CBCliente.SelectedValue.ToString();
            MostrarDeuda();
        }

        #endregion

        #region Forma de pago

        private void FormaDePago()
        {
            DataSet ds = new DataSet();
            ds = objNegFormaDePago.ListandoFormaDePago("Todos");
            CbFPago.DisplayMember = "Nombre_fpago";
            CbFPago.ValueMember = "Id_fpago";
            CbFPago.DataSource = ds.Tables[0];
        }

        private void CbFPago_SelectedIndexChanged(object sender, EventArgs e)
        {
            CbFPago.SelectedValue.ToString();

            if (CbFPago.Text == "Efectivo" || CbFPago.Text == "Débito")
            {
                txtNOp.Enabled = false;
            }
            else
            {
                txtNOp.Enabled = true;
            }

            //if (CbFPago.Text == "Tarjeta de crédito")
            //{
            //    txtRecargo.Text = "10";
            //}
            //else
            //{
            //    txtRecargo.Text = "0";
            //}
        }

        private void CrearColumnasPago()
        {
            DGVmdp.ColumnCount = 3;
            DGVmdp.Columns[0].HeaderText = "Id";
            DGVmdp.Columns[1].HeaderText = "Forma de pago";
            //DGVmdp.Columns[2].HeaderText = "Interés";
            DGVmdp.Columns[2].HeaderText = "Monto";
            //DGVmdp.Columns[4].HeaderText = "Monto con int";

            DGVmdp.Columns[0].Visible = false;
        }

        private void btnAceptarMdePagos_Click(object sender, EventArgs e)
        {
            if (!camposVaciosPago())
            {
                decimal total = 0;

                foreach (DataGridViewRow row in DGVmdp.Rows)
                {
                    if (CbFPago.Text == "Efectivo" || CbFPago.Text == "Débito" || CbFPago.Text == "Tarjeta de crédito")
                    {
                        total += Convert.ToDecimal(row.Cells[2].Value);
                        preciototal = total;
                    }
                    //else
                    //{
                    //    CalcularInteres();
                    //    total = Convert.ToDecimal(montoConInt);
                    //    preciototal = Convert.ToDecimal(montoConInt);
                    //}
                }

                DGVmdp.Rows.Add(CbFPago.SelectedValue, CbFPago.Text, txtMonto.Text, preciototal);

                Pagado = 0;
                CalcularTotalPagado();
                txtPagado.Text = Pagado.ToString();
                CalcularSaldo();
                CalcularVuelto();
            }

        }

        private void LimpiarCampos()
        {
            txtMonto.Text = "0";
            txtNOp.Text = string.Empty;
            txtVuelto.Text = "0";
            txtPagado.Text = "0";
            TxtCantidad.Text = "1";
            txtPDeuda.Text = "0";
            txtDescripcion.Text = string.Empty;
            txtPrecioPV.Text = "0";
            CBCliente.SelectedIndex = 0;
            txtADeuda.Text = "0";
        }
        //private void CalcularInteres()
        //{
        //    montoConInt = (double.Parse(txtMonto.Text) * double.Parse(txtRecargo.Text)) / 100;
        //}
        private void CalcularTotalPagado()
        {
            for (int i = 0; i < DGVmdp.Rows.Count; ++i)
            {
                Pagado += Convert.ToDecimal(DGVmdp.Rows[i].Cells[2].Value);
            }
        }

        private void CalcularSaldo()
        {
            decimal saldo = Convert.ToDecimal(lbltotal.Text) - Pagado;
            if (saldo <= 0)
            {
                txtADeuda.Text = saldo.ToString();
                CalcularVuelto();
            }
            else
            {
                txtADeuda.Text = saldo.ToString();
            }
        }

        private void CalcularVuelto()
        {
            decimal vuelto = Convert.ToDecimal(txtPagado.Text) - Convert.ToDecimal(lbltotal.Text);

            if (vuelto <= 0)
            {
                txtVuelto.Text = "0";
                txtVuelto.ForeColor = Color.Red;
                txtADeuda.Text = vuelto.ToString();
                txtADeuda.ForeColor = Color.Red;
            }
            else
            {
                txtVuelto.Text = vuelto.ToString();
                txtVuelto.ForeColor = Color.Green;
            }
        }

        #endregion

        #region ProductoVenta

        private void GuardarProductoVenta()
        {
            try
            {
                DataSet ds = new DataSet();
                ds = objNegVenta.UltimoRegistroVenta();

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        objEProductoVenta.Id_venta = Convert.ToInt32(dr["Id_venta"]);
                    }
                }

                for (int i = 0; i < DGVListaVenta.Rows.Count - 1; i++)
                {
                    objEProductoVenta.Preciou_historico = Convert.ToInt32(DGVListaVenta.Rows[i].Cells[3].Value);
                    objEProductoVenta.Monto = Convert.ToInt32(DGVListaVenta.Rows[i].Cells[4].Value);

                    objNegProductoVenta.InsertandoProductoVenta("Alta", objEProductoVenta);
                }

                MessageBox.Show("ProductoVenta guardado");

            }
            catch (Exception ex)
            {

                MessageBox.Show("No se pudo guardar el Producto Venta" + ex);
            }
        }

        private void ProdPan()
        {
            for (int i = 0; i < DGVListaVenta.Rows.Count - 1; i++)
            {
                objEProductoVenta.Id_producto = Convert.ToInt32(DGVListaVenta.Rows[i].Cells[0].Value);
                objEProductoVenta.Cantidad = Convert.ToInt32(DGVListaVenta.Rows[i].Cells[1].Value);
                objEProductoVenta.Preciou_historico = Convert.ToInt32(DGVListaVenta.Rows[i].Cells[3].Value);
                objEProductoVenta.Monto = Convert.ToInt32(DGVListaVenta.Rows[i].Cells[4].Value);
            }
        }

        #endregion

        #region Productos de Panaderia 
        private void btnAcepProdVarios_Click(object sender, EventArgs e)
        {
            AgregarProdPanaderia();
            preciototal = 0;
            CalcularTotalVenta();
            lbltotal.Text = preciototal.ToString();
        }

        private void AgregarProdPanaderia()
        {
            DGVListaVenta.Rows.Add(999, 1, txtDescripcion.Text, txtPrecioPV.Text, txtPrecioPV.Text);
        }
        #endregion

        #region Deuda
        private bool MostrarDeuda()
        {
            bool Tienedeuda = false;

            DataSet ds = new DataSet();
            ds = objNegDeuda.ListandoDeudasPorCliente(CBCliente.SelectedValue.ToString());
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Tienedeuda = true;
                    CBProducto.SelectedValue.ToString();
                    lblDeuda.Text = (dr["Importe"].ToString());

                    return Tienedeuda;
                }
            }

            return Tienedeuda;
        }

        private void BtnCobrarDeuda_Click(object sender, EventArgs e)
        {
            if (Convert.ToDecimal(txtVuelto.Text) == 0)
            {
                ValidacionPagoDeuda();
                CobrarDeudaSola();
            }
            else
            {
                CobrarActualizarDeuda();
            }
        }

        private void GuardarDeuda()
        {
            decimal adeuda = Convert.ToDecimal(txtADeuda.Text);
            decimal deudanueva = adeuda + Convert.ToDecimal(lblDeuda.Text);

            if (!MostrarDeuda())
            {
                try
                {
                    DataSet ds = new DataSet();
                    ds = objNegVenta.UltimoRegistroVenta();

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            objEDeuda.Id_venta1 = Convert.ToInt32(dr["Id_venta"]);
                        }
                    }
                    objEDeuda.Fecha1 = DateTime.Now.ToString("d");
                    objEDeuda.Id_cliente1 = Convert.ToInt32(CBCliente.SelectedValue.ToString());
                    objEDeuda.Importe1 = Convert.ToInt32(deudanueva);

                    objNegDeuda.InsertandoDeuda("Alta", objEDeuda);

                    MessageBox.Show("Deuda actualizada");
                    MostrarDeuda();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No se pudo actualizar la deuda" + ex);
                }
            }
            else
            {
                try
                {
                    DataSet ds = new DataSet();
                    ds = objNegVenta.UltimoRegistroVenta();

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            objEDeuda.Id_venta1 = Convert.ToInt32(dr["Id_venta"]);
                        }
                    }
                    objEDeuda.Fecha1 = DateTime.Now.ToString("d");
                    objEDeuda.Id_cliente1 = Convert.ToInt32(CBCliente.SelectedValue.ToString());
                    objEDeuda.Importe1 = Convert.ToInt32(deudanueva);

                    objNegDeuda.InsertandoDeuda("Modificar", objEDeuda);

                    MessageBox.Show("Deuda actualizada");
                    MostrarDeuda();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No se pudo actualizar la deuda" + ex);
                }
            }
        }

        private void CobrarDeudaSola()
        {
            decimal diferencia = 0;
            decimal deuda1 = Convert.ToDecimal(lblDeuda.Text);
            if (deuda1 < 0)
            {
                deuda1 *= -1;
            }

            ValidacionPagoDeuda(); //QUEDÉ ACÁ

            if (Convert.ToDecimal(txtVuelto.Text) == 0)
            {
                decimal MontoPagoDeuda;
                MontoPagoDeuda = Convert.ToDecimal(txtPDeuda.Text);
                diferencia = deuda1 - MontoPagoDeuda;
            }

            try
            {
                objEDeuda.Id_cliente1 = Convert.ToInt32(CBCliente.SelectedValue.ToString());
                objEDeuda.Importe1 = Convert.ToInt32(diferencia);

                objNegDeuda.EditandoDeuda("Modificar", objEDeuda);

                MessageBox.Show("Deuda actualizada");
                MostrarDeuda();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo actualizar la deuda" + ex);
            }
        }

        private void CobrarActualizarDeuda()
        {
            decimal diferencia = 0;
            decimal deuda1 = Convert.ToDecimal(lblDeuda.Text);
            deuda1 = deuda1 * -1;

            if (Convert.ToDecimal(txtVuelto.Text) > 0)
            {
                diferencia = Convert.ToDecimal(txtVuelto.Text) - deuda1;

                if (diferencia >= 0)
                {
                    txtVuelto.Text = diferencia.ToString();
                    diferencia = 0;
                }
            }

            try
            {
                objEDeuda.Id_cliente1 = Convert.ToInt32(CBCliente.SelectedValue.ToString());
                objEDeuda.Importe1 = Convert.ToInt32(diferencia);

                objNegDeuda.EditandoDeuda("Modificar", objEDeuda);

                MessageBox.Show("Deuda actualizada");
                MostrarDeuda();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo actualizar la deuda" + ex);
            }
        }

        #endregion

        #region Caja

        private void CrearColumnasCaja()
        {
            DgvCaja.ColumnCount = 9;
            DgvCaja.Columns[0].HeaderText = "Id";
            DgvCaja.Columns[1].HeaderText = "Cliente";
            DgvCaja.Columns[2].HeaderText = "Autorizado";
            DgvCaja.Columns[3].HeaderText = "Forma de pago";
            DgvCaja.Columns[4].HeaderText = "Monto";
            DgvCaja.Columns[5].HeaderText = "Fecha";
            DgvCaja.Columns[6].HeaderText = "Hora";
            DgvCaja.Columns[7].HeaderText = "Estado";
            DgvCaja.Columns[8].HeaderText = "N° de Factura";
        }

        private void CrearColumnasEgresosCaja()
        {
            dgvCajaEgresos.ColumnCount = 8;
            dgvCajaEgresos.Columns[0].HeaderText = "Id";
            dgvCajaEgresos.Columns[1].HeaderText = "Proveedor";
            dgvCajaEgresos.Columns[2].HeaderText = "Autorizado";
            dgvCajaEgresos.Columns[3].HeaderText = "Forma de pago";
            dgvCajaEgresos.Columns[4].HeaderText = "Monto";
            dgvCajaEgresos.Columns[5].HeaderText = "Fecha";
            dgvCajaEgresos.Columns[6].HeaderText = "Estado";
            dgvCajaEgresos.Columns[7].HeaderText = "N° de Factura";
        }
        private void LlenarDGVCaja()
        {
            DgvCaja.Rows.Clear();
            DataSet ds = new DataSet();
            ds = objNegVenta.ListandoVentas("TodosDetalle");
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DgvCaja.Rows.Add(dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), dr[6].ToString(), dr[7].ToString(), dr[8].ToString());
                }
            }
        }

        private void LlenarCajaEgresos()
        {
            dgvCajaEgresos.Rows.Clear();
            DataSet ds = new DataSet();
            ds = objNegCompra.ListandoCompras("TodoCompras");
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    dgvCajaEgresos.Rows.Add(dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), dr[6].ToString(), dr[7].ToString());
                }
            }
        }

        private void LlenarDgvVentasPorFecha()
        {
            DataSet ds = new DataSet();
            ds = objNegVenta.VentasEntre(txtDesde.Text, txtHasta.Text);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DgvCaja.Rows.Add(dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), dr[6].ToString(), dr[7].ToString(), dr[8].ToString());
                }
            }
        }

        private void LlenarCajaEgresosPorFecha()
        {
            DataSet ds = new DataSet();
            ds = objNegCompra.ComprasEntre(txtDesdeC.Text, txtHastaC.Text);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    dgvCajaEgresos.Rows.Add(dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), dr[6].ToString(), dr[7].ToString());
                }
            }
        }
        private void btnBuscarPorFecha_Click(object sender, EventArgs e)
        {
            DgvCaja.Rows.Clear();
            CrearColumnasCaja();
            LlenarDgvVentasPorFecha();
        }

        private void txtBFecha_Click(object sender, EventArgs e)
        {
            dgvCajaEgresos.Rows.Clear();
            LlenarCajaEgresosPorFecha();
        }

        private void btnCerrarCaja_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Cerrar caja con un monto de $" + txtMontoInicial.Text + " ?", "Cerrar caja", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                int nGrabados = -1;

                objECaja.ImporteFinal1 = Convert.ToDecimal(txtMontoInicial.Text);
                objECaja.Estado1 = false;
                objECaja.Fecha1 = DateTime.Now.ToString("d");
                nGrabados = objNegCaja.abmCaja("Cierre", objECaja);

                if (nGrabados == -1)
                {
                    MessageBox.Show(" No se pudo cerrar la caja");
                }
                else
                {
                    MessageBox.Show("Cierre de caja realizado");
                }
            }
        }

        #endregion

        #region Busquedas rápidas
        private void tbBuscarClientes_TextChanged(object sender, EventArgs e)
        {
            dgvCliente.Rows.Clear();
            DataSet ds = new DataSet();
            ds = objNegCliente.ListadoClientesRapido(tbBuscarClientes.Text);

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                dgvCliente.Rows.Add(dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), dr[6].ToString());
            }
        }

        private void tbBuscarAutorizados_TextChanged(object sender, EventArgs e)
        {
            dgvAutorizados.Rows.Clear();
            DataSet ds = new DataSet();
            ds = objNegAutorizado.ListadoAutorizadoRapido(tbBuscarAutorizados.Text);

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                dgvAutorizados.Rows.Add(dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString());
            }
        }

        private void tbBuscarProductos_TextChanged(object sender, EventArgs e)
        {
            dgvProductos.Rows.Clear();
            DataSet ds = new DataSet();
            ds = objNegProducto.ListadoProductoRapido(tbBuscarProductos.Text);

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                dgvProductos.Rows.Add(dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), dr[6].ToString());
            }
        }

        private void tbBuscarCategorias_TextChanged(object sender, EventArgs e)
        {
            dgvCategorias.Rows.Clear();
            DataSet ds = new DataSet();
            ds = objNegCategoria.ListadoCategoriaRapido(tbBuscarCategorias.Text);

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                dgvCategorias.Rows.Add(dr[0].ToString(), dr[1].ToString(), dr[2].ToString());
            }
        }

        private void txtbuscar_TextChanged(object sender, EventArgs e)
        {
            dgvProv.Rows.Clear();
            DataSet ds = new DataSet();
            ds = objNegProveedor.ListadoProveedorRapido(txtbuscar.Text);

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                dgvProv.Rows.Add(dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), dr[6].ToString());
            }
        }

        #endregion

        #region Eliminar Producto de la venta
        private void BTRemover_Click(object sender, EventArgs e)
        {
            decimal total = 0;
            decimal pagado = 0;
            int fila = DGVListaVenta.CurrentRow.Index;
            DGVListaVenta.Rows.RemoveAt(fila);

            foreach (DataGridViewRow row in DGVListaVenta.Rows)
            {
                total += Convert.ToDecimal(row.Cells[4].Value);
            }
            lbltotal.Text = Convert.ToString(total);

            total = decimal.Parse(lbltotal.Text);
            pagado = decimal.Parse(txtPagado.Text);

            if (pagado < total)
            {
                txtADeuda.Text = Convert.ToString(total - pagado);
            }
            if (pagado == total)
            {
                txtADeuda.Text = Convert.ToString(total - pagado);
            }
            if (pagado > total)
            {
                txtADeuda.Text = Convert.ToString(total - pagado);
            }
            if (txtADeuda.Text == "0")
            {
                txtADeuda.BackColor = Color.WhiteSmoke;
            }
        }
        #endregion

        #region Eliminar Pagos
        private void btnETodosMP_Click(object sender, EventArgs e)
        {
            DGVmdp.Rows.Clear();
            txtPagado.Text = "0";
            txtADeuda.Text = "0";
            txtMonto.Text = string.Empty;
            // txtRecargo.Text = string.Empty;
        }

        private void btnEliminarPago_Click(object sender, EventArgs e)
        {
            decimal importe = 0;
            decimal total = 0;

            int fila = DGVmdp.CurrentRow.Index;
            DGVmdp.Rows.RemoveAt(fila);

            foreach (DataGridViewRow row in DGVmdp.Rows)
            {
                importe += Convert.ToDecimal(row.Cells[3].Value);
            }
            Pagado = importe;
            txtADeuda.Text = Convert.ToString(decimal.Parse(lbltotal.Text) - Pagado);

            txtPagado.Text = Convert.ToString(importe);
            txtADeuda.Text = Convert.ToString(decimal.Parse(lbltotal.Text) - decimal.Parse(txtPagado.Text));

            total = decimal.Parse(lbltotal.Text);
            decimal pagado = decimal.Parse(txtPagado.Text);

            if (pagado < total)
            {
                txtADeuda.Text = Convert.ToString(total - pagado);
            }
            if (pagado == total)
            {
                txtADeuda.Text = Convert.ToString(total - pagado);
            }
            if (pagado > total)
            {
                txtADeuda.Text = Convert.ToString(total - pagado);
            }
            if (txtADeuda.Text == "0")
            {
                txtADeuda.BackColor = Color.WhiteSmoke;
            }

        }

        #endregion

        #region ticket
        private void Imprimir(object sender, PrintPageEventArgs e)
        {
            Font font = new Font("Arial", 14);
            int ancho = 500;
            int y = 30;

            e.Graphics.DrawString(" ***** PANADERÍA PANARTE ***** ", font, Brushes.Black, new RectangleF(0, y += 30, ancho, 20));
            e.Graphics.DrawString(" Cliente: " + CBCliente.Text, font, Brushes.Black, new RectangleF(0, y += 30, ancho, 20));
            e.Graphics.DrawString(" -------- Productos ---------- " + CBCliente.Text, font, Brushes.Black, new RectangleF(0, y += 30, ancho, 20));

            foreach (DataGridViewRow row in DGVListaVenta.Rows)
            {
                e.Graphics.DrawString((row.Cells[2].Value) + "     " + (row.Cells[4].Value), font, Brushes.Black, new RectangleF(0, y += 30, ancho, 20));
            }

            e.Graphics.DrawString(" Total: $" + lbltotal.Text, font, Brushes.Black, new RectangleF(0, y += 40, ancho, 20));

            e.Graphics.DrawString(" *** GRACIAS POR SU COMPRA *** ", font, Brushes.Black, new RectangleF(0, y += 30, ancho, 20));
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            printDocument1 = new PrintDocument();
            PrinterSettings ps = new PrinterSettings();
            printDocument1.PrinterSettings = ps;
            printDocument1.PrintPage += Imprimir;
            printDocument1.Print();
        }

        #endregion

        #region compra
        private void LlenarCbProveedores()
        {
            DataSet ds = new DataSet();
            ds = objNegProveedor.ListandoProveedor("Todos");
            CbProveedores.DisplayMember = "nombre_prov";
            CbProveedores.ValueMember = "id_prov";
            CbProveedores.DataSource = ds.Tables[0];
        }

        private void LlenarCbProd()
        {
            DataSet ds = new DataSet();
            ds = objNegProducto.ListandoProductos("Todos");
            CbProductos.DisplayMember = "Nombre_producto";
            CbProductos.ValueMember = "Id_producto";
            CbProductos.DataSource = ds.Tables[0];
        }
        private void CrearColumnasCompra()
        {
            dgvCompras.ColumnCount = 7;

            dgvCompras.Columns[0].HeaderText = "Proveedor";
            dgvCompras.Columns[1].HeaderText = "Producto";
            dgvCompras.Columns[2].HeaderText = "Cantidad";
            dgvCompras.Columns[3].HeaderText = "Precio";
            dgvCompras.Columns[4].HeaderText = "N° de Factura";
            dgvCompras.Columns[5].HeaderText = "Precio Final";
        }

        private void btnGuardarCompra_Click(object sender, EventArgs e)
        {
            GuardarCompra();
        }

        private void FormaDePagoC()
        {
            DataSet ds = new DataSet();
            ds = objNegFormaDePago.ListandoFormaDePago("Todos");
            cbfp.DisplayMember = "Nombre_fpago";
            cbfp.ValueMember = "Id_fpago";
            cbfp.DataSource = ds.Tables[0];
        }

        private void GuardarCompra()
        {
            try
            {
                objECompra.Id_Proveedor1 = Convert.ToInt32(CbProveedores.SelectedValue.ToString());
                objECompra.Id_Autorizado1 = IdAutorizado;
                objECompra.Id_Fpago1 = Convert.ToInt32(cbfp.SelectedValue.ToString());
                objECompra.MontoFinal1 = preciototalC;
                objECompra.Fecha_Compra1 = DateTime.Now.ToString("d");
                objECompra.Estado1 = cbEst.Text;
                objECompra.N_Factura1 = txtNFact.Text;

                objNegCompra.InsertandoCompra("Alta", objECompra);
                GuardarProductoCompra();
                objNegProducto.SumarStock("SumaStock", objEProductoCompra);

                MessageBox.Show("Compra realizada con éxito");
            }
            catch (Exception)
            {
                MessageBox.Show("La compra no se pudo realizar");
            }
        }

        private void GuardarProductoCompra()
        {
            try
            {
                DataSet ds = new DataSet();
                ds = objNegCompra.UltimoRegistroCompra();

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        objEProductoCompra.Id_compra1 = Convert.ToInt32(dr["Id_compra"]);
                    }
                }

                for (int i = 0; i < dgvCompras.Rows.Count - 1; i++)
                {
                    objEProductoCompra.Id_producto1 = Convert.ToInt32(CbProductos.SelectedValue.ToString());
                    objEProductoCompra.Cantidad1 = Convert.ToInt32(dgvCompras.Rows[i].Cells[2].Value);
                    objEProductoCompra.Preciou_historico1 = Convert.ToInt32(dgvCompras.Rows[i].Cells[3].Value);

                    objNegProductoCompra.InsertandoProductoCompra("Alta", objEProductoCompra);
                }

                MessageBox.Show("Producto Compra guardado");

            }
            catch (Exception ex)
            {

                MessageBox.Show("No se pudo guardar el Producto compra" + ex);
            }
        }

        private void btnAgregarProd_Click(object sender, EventArgs e)
        {
            decimal CostoxCant = Convert.ToInt32(txtCantCompra.Text) * Convert.ToInt32(txtPCompra.Text);

            dgvCompras.Rows.Add(Convert.ToInt32(CbProveedores.SelectedValue.ToString()), Convert.ToInt32(CbProductos.SelectedValue.ToString()), txtCantCompra.Text, txtPCompra.Text, txtNFact.Text, CostoxCant);

            preciototalC = 0;
            CalcularTotalCompra();
        }

        private void CalcularTotalCompra()
        {
            for (int i = 0; i < dgvCompras.Rows.Count; ++i)
            {
                preciototalC += Convert.ToDecimal(dgvCompras.Rows[i].Cells[5].Value);
            }
            lblTCompra.Text = preciototalC.ToString();
        }


        #endregion

        #region validaciones

        private void txtMonto_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private bool camposVaciosProducto()
        {
            bool correcta = true;
            if (TxtCantidad.Text == string.Empty || Convert.ToDecimal(TxtCantidad.Text) <= 0)
            {
                correcta = false;
                MessageBox.Show("La cantidad de productos no puede ser menor a 1");
                TxtCantidad.SelectAll();
                TxtCantidad.Focus();
            }

            return correcta;
        }

        private bool camposVaciosPago()
        {
            bool validation = false;

            if (!txtMonto.Text.All(char.IsNumber) || txtMonto.Text == string.Empty)
            {
                validation = true;
                MessageBox.Show("Debe ingresar un monto de pago correcto");
                txtMonto.SelectAll();
                txtMonto.Focus();
                return validation;
            }

            if (CbFPago.SelectedValue.ToString() == string.Empty)
            {
                validation = true;
                MessageBox.Show("Debe ingresar una forma de pago correcta");
                CbFPago.Focus();
                return validation;
            }

            if (CbFPago.SelectedValue.ToString() == "Tarjeta de crédito")
            {
                validation = true;
                MessageBox.Show("Debe ingresar un numero de operacion correcto");
                txtNOp.SelectAll();
                txtNOp.Focus();
                return validation;
            }

            return validation;
        }

        private bool ValidacionPagoDeuda()
        {
            bool validation = false;

            if (!txtPDeuda.Text.All(char.IsNumber) || txtPDeuda.Text == string.Empty)
            {
                validation = true;
                MessageBox.Show("Debe ingresar un monto de pago correcto");
                txtMonto.SelectAll();
                txtMonto.Focus();
                return validation;
            }
            else if (txtPDeuda.Text == string.Empty || Convert.ToDecimal(txtPDeuda.Text) <= 0)
            {
                validation = false;
                MessageBox.Show("El pago no puede ser menor a 1");
                txtPDeuda.SelectAll();
                txtPDeuda.Focus();
                return validation;
            }
            return validation;
        }

        private void TxtCantidad_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (Convert.ToDecimal(TxtCantidad.Text) < 0)
            {
                MessageBox.Show("La cantidad no debe ser menor que 1");
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtPDeuda_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtPrecioPV_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        #endregion

        private void BTCancelar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

    }
}