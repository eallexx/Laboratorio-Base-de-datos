using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Laboratorio4
{
    public partial class Form1 : Form
    {
        private List<Producto> listaProductos;
        private Dictionary<string, object> myProducto = new Dictionary<string, object>();
        int idProducto;
        bool todoOK = true;
         List<(TextBox txt, IvalidadotorCampo validador)> camposValidar = new List<(TextBox txt, IvalidadotorCampo validador)>();
        public Form1()
        {
            InitializeComponent();
            listaProductos = new List<Producto>();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cargarProductos();
        }

        private void cargarProductos(string filtro = "")
        {
            DGV.Rows.Clear();
            DGV.Refresh();
            listaProductos = Conexion.GetProductos(filtro);

            foreach (var prod in listaProductos)
            {
                Image img = null;

                if (prod.Imagen != null && prod.Imagen.Length > 0)
                {
                    using (MemoryStream ms = new MemoryStream(prod.Imagen))
                    {
                        using (Bitmap bmp = new Bitmap(ms))
                        {
                            img = new Bitmap(bmp);
                        }
                            
                    }
                }
                DGV.Rows.Add(prod.Id, prod.Nombre, prod.Precio, prod.Cantidad, img);
            }//fin del foreach listaProductos
        }//fin de cargarProductos

        private void txtBusqueda_TextChanged(object sender, EventArgs e)
        {
            cargarProductos(txtBusqueda.Text.Trim());
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Seleccionar imagen del producto";
                openFileDialog.Filter = "Archivos de imagen|*.jpg;*.jpeg;*png;*bmp";

                if(openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Carga la imagen seleccioanda en el PictureBox y austa su tamaño
                    pictureBox.Image = Image.FromFile(openFileDialog.FileName);
                    pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                }
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (!datosCorrectos())
            {
                return; //No vamos hacer nada - se detiene en este punto, puedes crear un punto de interrupcion
            }

            CargarDatosProductos();

            if(Conexion.InsertSeguro("productos", myProducto))
            {
                MessageBox.Show("Se ha guardad satisfactoriamente el registro");
                //Aqui refrescas el grid volviendo a consultar la base ded datos
                cargarProductos();
            }//fin del if InsertSeguro
        }

        private void CargarDatosProductos()
        {
            myProducto["Cantidad"] = int.Parse(txtCantidad.Text.Trim());
            myProducto["Precio"] = decimal.Parse(txtPrecio.Text.Trim());
            myProducto["Nombre"] = txtNombre.Text.Trim();
            myProducto["Imagen"] = ImageToByteArray(pictureBox.Image);
        }

        private byte[] ImageToByteArray(Image image)
        {
            if (image == null)
                return null;

            using (MemoryStream mMemoryStream = new MemoryStream())
            {
                //Guardamos la imagen usando su formato original (RawFormat)
                image.Save(mMemoryStream, image.RawFormat);
                return mMemoryStream.ToArray();
            }

        }
        private bool datosCorrectos()
        {
            camposValidar.Add((txtNombre, new ValidatorTexto()));
            camposValidar.Add((txtPrecio, new ValidadorDecimal()));
            camposValidar.Add((txtCantidad, new ValidadorEntero()));

            foreach(var item in camposValidar)
            {
                if (!item.validador.EsValido(item.txt.Text))
                {
                    errorProvider1.SetError(item.txt, item.validador.MensajeError);
                    todoOK = false;
                    break;
                        
                }
                else
                {
                    errorProvider1.SetError(item.txt, string.Empty);
                    todoOK = true;
                }
            }
            return todoOK;
        }

        private void DGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow fila = DGV.Rows[e.RowIndex];
            idProducto = 0;
            
            idProducto = Convert.ToInt32(fila.Cells["Folio"].Value);
            txtNombre.Text = Convert.ToString(fila.Cells["Nombre"].Value);
            txtPrecio.Text = Convert.ToDecimal(fila.Cells["Precio"].Value).ToString();
            txtCantidad.Text = Convert.ToInt32(fila.Cells["Cantidad"].Value).ToString();

            btnAgregar.Enabled = false;
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            ModificarDatosBD();
        }

        private void ModificarDatosBD()
        {
            CargarDatosProductos();
            //Asegurar que mi arreglo tiene los datos
            //MessageBox.Show("el Nombre del producto es: " + myProducto["Nombre"]);
            //Actualiza el producto donde el id_producto sea igual a 5

            MessageBox.Show("el id del producto es: " + idProducto);


            
            bool resultado = Conexion.UpdateSeguro("productos", myProducto, "id", idProducto);

            if (resultado)
            {
                Console.WriteLine("Actualizacion exitosa.");
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            limpiarCampos();
            btnModificar.Enabled = false;
        }//btnLimpiar_clic

        public void limpiarCampos()
        {
            txtFolio.Text = "";
            txtNombre.Text = "";
            txtPrecio.Text = "";
            txtCantidad.Text = "";
            pictureBox.Image = null;
            btnAgregar.Enabled = true;
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
