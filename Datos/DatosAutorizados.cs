﻿using System;
using System.Data.SqlClient;
using System.Data;
using Entidades;

namespace Datos
{
    public class DatosAutorizados : DatosConexionDB
    {
        public DataSet listadoAutorizados(string cual)
        {
            string orden;
            if (cual != "Todos")

                orden = "select * from Autorizado where Id_autorizado = " + int.Parse(cual) + ";";
            else
                orden = "select * from Autorizado;";
            SqlCommand cmd = new SqlCommand(orden, Conexion);
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                Conexion.Open();
                cmd.ExecuteNonQuery();
                da.SelectCommand = cmd;
                da.Fill(ds);
            }
            catch (Exception e)
            {
                throw new Exception("Error al listar Autorizados", e);
            }
            finally
            {
                Conexion.Close();
                cmd.Dispose();
            }
            return ds;
        }

        public int AbmAutorizados(string accion, E_Autorizados objEAutorizado)
        {
            int resultado = -1;
            string orden = string.Empty;

            if (accion == "Alta")
            {
                orden = "insert into Autorizado values ('" + objEAutorizado.Nombre_aut +
                    "','" + objEAutorizado.Apellido_aut +
                    "','" + objEAutorizado.Usuario_aut +
                     "','" + objEAutorizado.Clave_aut +
                    "','" + objEAutorizado.Esta_cancelado + "');";
            }

            if (accion == "Modificar")
            {
                orden = "update Autorizado set Nombre_autorizado = '" + objEAutorizado.Nombre_aut +
                    "', Apellido_autorizado = '" + objEAutorizado.Apellido_aut +
                    "', Usuario_autorizado = '" + objEAutorizado.Usuario_aut +
                    "', Clave_autorizado = '" + objEAutorizado.Clave_aut +
                    "', esta_cancelado = '" + objEAutorizado.Esta_cancelado +
                    "'where Id_autorizado = " + objEAutorizado.Id + ";";
            }

            if (accion == "Eliminar")
            {
                orden = "Update Autorizado set esta_cancelado = '" + objEAutorizado.Esta_cancelado + "' where Id_autorizado = " + objEAutorizado.Id + ";";
            }

            SqlCommand cmd = new SqlCommand(orden, Conexion);

            try
            {
                AbrirConexion();
                resultado = cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {

                throw new Exception("Error al tratar de eliminar el autorizado", e);
            }
            finally
            {
                CerrarConexion();
                cmd.Dispose();
            }

            return resultado;
        }

        public int Login(E_Autorizados objEAutorizado)
        {
            string orden = "SELECT Id_autorizado, Usuario_autorizado, Clave_autorizado FROM Autorizado WHERE Usuario_autorizado = '" + objEAutorizado.Usuario_aut + "'AND Clave_autorizado ='" + objEAutorizado.Clave_aut + "' ";
            SqlCommand cmd = new SqlCommand(orden, Conexion);
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            int id_Autorizado = 0;

            try
            {
                Conexion.Open();
                cmd.ExecuteNonQuery();
                da.SelectCommand = cmd;
                da.Fill(ds);


                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    id_Autorizado = int.Parse(dr[0].ToString());
                }
            }

            catch (Exception e)
            {
                throw new Exception("Usuario no encontrado", e);
            }
            finally
            {
                Conexion.Close();
                cmd.Dispose();
            }


            return id_Autorizado;
        }

        public DataSet ListadoAutorizadoRapido(string cual)
        {
            string orden = string.Empty;
            if (cual != "Todos")
            {
                orden = "select * from Autorizado where Nombre_autorizado like '%" + cual + "%' or Apellido_autorizado like '" + cual + "%';";
            }
            else
            {
                orden = "select * from Autorizado;";
            }
            SqlCommand cmd = new SqlCommand(orden, Conexion);
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();

            try
            {
                Conexion.Open();
                cmd.ExecuteNonQuery();

                da.SelectCommand = cmd;
                da.Fill(ds);
            }
            catch (Exception e)
            {
                throw new Exception("Error al listar Autorizados", e);
            }
            finally
            {
                Conexion.Close();
                cmd.Dispose();
            }
            return ds;
        }

        public DataSet listadoAutorizadoPorFecha(string fecha)
        {
            string orden = "select * from caja where fecha = '" + fecha + "' and Estado = 1";

            SqlCommand cmd = new SqlCommand(orden, Conexion);
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                Conexion.Open();
                cmd.ExecuteNonQuery();
                da.SelectCommand = cmd;
                da.Fill(ds);
            }
            catch (Exception e)
            {
                throw new Exception("Error al listar Autorizados", e);
            }
            finally
            {
                Conexion.Close();
                cmd.Dispose();
            }
            return ds;
        }
    }
}
