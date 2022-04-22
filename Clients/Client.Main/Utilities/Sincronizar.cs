using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Main.Utilities
{
    public class Sincronizar : Screen
    {
        public static Connect conexion = ContainerConfig.scope.Resolve<Connect>();

        public static async Task<bool> SincronizarRegistro()
        {
            try
            {
                ///Obtiene de la base de datos en el servidor la informacion de los registros que han cambiado y deben ser actualizados en la base local para el evento en que el cliente deba funcionar localmente.
                Task<object> registros = conexion.CallServerMethod("ServidorNuevosRegistros", Arguments: new object[] { DbConnection.ultimoRegistro() });
                await registros;
                BindableCollection<string[]> registroDeCambios = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<string[]>>(registros.Result.ToString());

                foreach (string[] registro in registroDeCambios)
                {
                    if (registro[1] == "Insert" | registro[1] == "Update")
                    {
                        ///Se llama al metodo especificado en los detalles en la posicion uno del arreglo.
                        Task<object> re = conexion.CallServerMethod(registro[2], Arguments: new[] { registro[3] });

                        ///Espera asicronicamente que la llamada al metodo del servidor echa anteriormente devuelva una respuesta.
                        await re;

                        ///Deserializa el Json que da como respuesta la llamada al metodo.
                        dynamic[] resultado = (dynamic[])System.Text.Json.JsonSerializer.Deserialize(re.Result.ToString(), Type.GetType("Client.Main.Models." + registro[4]));

                        ///Crea una instancia de la clase DbConnection que permitira el uso de la refelxion para llamar a los metodos necesarios.
                        DbConnection conexionLocal = new DbConnection();

                        ///Obtiene la informacion del metodo especificado en los detalles, que hace parte de la clase DbConnection y por tanto de la instancia conexionLocal.
                        MethodInfo mi = conexionLocal.GetType().GetMethod(registro[5]);

                        ///Instancia que se retornara como correcta
                        object instanciaCorrecta;


						foreach (var item in resultado)
						{
							///Se comprueba la clave primaria.
							if (item.GetType().GetProperty(registro[6].Trim()).GetValue(item) == registro[3])
							{
								instanciaCorrecta = item;
								///Invoca el metodo con el resultado del llamdo al metodo del servidor.
								if ((bool)mi.Invoke(conexionLocal, new object[] { instanciaCorrecta }))
								{
									///Actualiza en la base de datos local el ID del ultimo registro sincronizado.
									DbConnection.actualizarUltimoRegistro(int.Parse(registro[0]));
								}
								break;
							}
						}
                    }
                    if (registro[1] == "Delete")
                    {
                        ///Crea una instancia de la clase DbConnection que permitira el uso de la refelxion para llamar a los metodos necesarios.
                        DbConnection conexionLocal = new DbConnection();

                        ///Obtiene la informacion del metodo especificado en los detalles, que hace parte de la clase DbConnection y por tanto de la instancia conexionLocal.
                        MethodInfo mi = conexionLocal.GetType().GetMethod(registro[5]);

                        ///Invoca el metodo con el resultado del llamdo al metodo del servidor.
                        if ((bool)mi.Invoke(conexionLocal, new object[] { registro[3] }))
                        {
                            ///Actualiza en la base de datos local el ID del ultimo registro sincronizado.
                            DbConnection.actualizarUltimoRegistro(int.Parse(registro[0]));
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

                throw;
            }

        }
    }
}


