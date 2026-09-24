using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laboratorio4
{
    public class Conexion
    {
        private static string cadenaConexion = "Server=localhost; Database=productosdb;Uid=root;Pwd=Cambio23";
        public static MySqlConnection ObtenerConexion()
        {
            try
            {
                //Crear un tipo de dato de MySQLConnection 
                MySqlConnection conexion = new MySqlConnection(cadenaConexion);
                conexion.Open();
                return (conexion);
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error al conectar: " + ex.Message);
                return null;
            }//fin del catch 
        } // fin dle método estático MySqlConnection


        public static List<Producto> GetProductos(string filtro)
        {
            List<Producto> listaProductos = new List<Producto>();

            string query = "SELECT id, nombre, precio, cantidad, imagen FROM productos";

            // Si viene un filtro, modificamos el query de forma segura  
            // (Nota: idealmente con parámetros, pero adaptado al ejemplo visual que tienes) 
            if (!string.IsNullOrEmpty(filtro))
            {
                query += " WHERE id LIKE @filtro OR nombre LIKE @filtro " +
                " OR precio LIKE @filtro OR cantidad LIKE @filtro";
            }

            using (MySqlConnection conn = ObtenerConexion())
            {
                if (conn == null) return listaProductos;
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // Si hay filtro, agregamos el parámetro para evitar inyección SQL 
                    if (!string.IsNullOrEmpty(filtro))
                    {
                        cmd.Parameters.AddWithValue("@filtro", "%" + filtro + "%");
                    }

                    // Ejecutamos el lector de datos 
                    using (MySqlDataReader mReader = cmd.ExecuteReader())
                    {
                        // Recorremos el lector fila por fila mientras haya registros 
                        while (mReader.Read())
                        {
                            Producto prod = new Producto();
                            // Mapeamos los campos de la base de datos a las propiedades de tu clase Producto 
                            // (Ajusta los nombres de las columnas o índices según tu base de datos) 
                            prod.Id = Convert.ToInt32(mReader["id"]);
                            prod.Nombre = mReader["nombre"].ToString();
                            prod.Precio = Convert.ToDecimal(mReader["precio"]);
                            prod.Cantidad = Convert.ToInt32(mReader["cantidad"]);
                            //prod.Imagen = (byte[])mReader.GetValue(4); 
                            prod.Imagen = mReader["imagen"] != DBNull.Value ? (byte[])mReader["imagen"] : null;
                            // Agregamos el objeto listo a la lista genérica 
                            listaProductos.Add(prod);
                        }
                        mReader.Close();
                    }//MySqlDataReader 
                }// MySqlCommand 

            
            }
            return listaProductos;
        }// fin del metodo
        public static bool InsertSeguro(string tbName, Dictionary<string, object> data)
        {
            var columns = string.Join(", ", data.Keys);
            var placeholders = "@" + string.Join(", @", data.Keys);

            string sql = $"INSERT INTO {tbName} ({columns}) VALUES ({placeholders})";

            try
            {
                //pedimos la conexion usando nuestra clase externa
                using (MySqlConnection conexion = ObtenerConexion())
                {
                    if (conexion == null) return false;

                    using (MySqlCommand stmt = new MySqlCommand(sql, conexion))
                    {
                        foreach (var kvp in data)
                        {
                            stmt.Parameters.AddWithValue("@" + kvp.Key, kvp.Value ?? DBNull.Value);
                        }
                        stmt.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error en INSERT: " + ex.Message);
                return false;
            }

        }//fin del metoto Insertar Seguro

        public static bool UpdateSeguro(string tbName, Dictionary<string, object> data, string idColumn, int idValue)
        {
            //Construimos la clausula SET: columna 1, columna2 = @columna2...
            var setParts = new List<string>();
            foreach (var key in data.Keys)
            {
                setParts.Add($"{key} = @{key}");
            }
            string setClause = string.Join(", ", setParts);
            string sql = $"UPDATE {tbName} SET {setClause} where {idColumn} = @idCondicion";

            try
            {
                using (MySqlConnection conexion = ObtenerConexion())
                {
                    if (conexion == null) return false;

                    using (MySqlCommand stat = new MySqlCommand(sql, conexion))
                    {
                        //agregamos los parametros de los campos a actualizar
                        foreach (var kvp in data)
                        {
                            stat.Parameters.AddWithValue("@" + kvp.Key, kvp.Value ?? DBNull.Value);
                        }

                        //agregamos el parametro para la condicion WHERE de forma segura
                        stat.Parameters.AddWithValue("@idCondicion", idValue);

                        stat.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error en UPDATE: " + ex.Message);
                return false;
            }
        }
    }
}
    
    

    

