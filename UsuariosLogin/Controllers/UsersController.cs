using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using UsuariosLogin.Models;

namespace UsuariosLogin.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly string cadenaSQL;
        public UsersController(IConfiguration config)
        {
            cadenaSQL = config.GetConnectionString("CadenaSQL");
        }
        [HttpGet]
        [Route("List")]

        public IActionResult Listar()
        {
            List<User> lista = new List<User>();
            try
            {
                using (var conection = new SqlConnection(cadenaSQL))
                {
                    conection.Open();
                    var cmd = new SqlCommand("sp_GetAllLogger", conection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            lista.Add(new User()
                            {
                                id = Convert.ToInt32(rd["Id"]),
                                username = Convert.ToString(rd["Username"]),
                                password = Convert.ToString(rd["Passwords"])
                            });
                        }
                    }
                }
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "OK", response = lista });
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message, Response = lista });
            }
        }
        [HttpGet]
        [Route("Get/{Id:int}")]

        public IActionResult Obtener(int Id)
        {
            List<User> lista = new List<User>();
            User usuario = new User();
            try
            {
                using (var conection = new SqlConnection(cadenaSQL))
                {
                    conection.Open();
                    var cmd = new SqlCommand("sp_GetLoggerById", conection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            lista.Add(new User()
                            {
                                id = Convert.ToInt32(rd["Id"]),
                                username = Convert.ToString(rd["Username"]),
                                password = Convert.ToString(rd["Passwords"])
                            });
                        }
                    }
                }
                usuario = lista.Where(item => item.id == Id).FirstOrDefault();
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "OK", response = usuario });
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message, Response = lista });
            }
        }
        [HttpPost]
        [Route("Register")]
        public IActionResult Guardar([FromBody] User usuario)
        {
            try
            {
                using (var conection = new SqlConnection(cadenaSQL))
                {
                    conection.Open();
                    var cmd = new SqlCommand("sp_AddULogger", conection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("Username", usuario.username);
                    cmd.Parameters.AddWithValue("Passwords", usuario.password);
                    cmd.ExecuteNonQuery();
                }
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Registered" });
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });
            }
        }
        [HttpPut]
        [Route("Edit")]
        public IActionResult Editar([FromBody] User usuario)
        {
            try
            {
                using (var conection = new SqlConnection(cadenaSQL))
                {
                    conection.Open();
                    var cmd = new SqlCommand("sp_UpdateLogger", conection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("Id", usuario.id == 0 ? DBNull.Value : usuario.id);
                    cmd.Parameters.AddWithValue("Username", usuario.username is null ? DBNull.Value : usuario.username);
                    cmd.Parameters.AddWithValue("Passwords", usuario.password is null ? DBNull.Value : usuario.password);
                    cmd.ExecuteNonQuery();
                }
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Updated" });
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });
            }
        }
        [HttpPost]
        [Route("Delete/{ID:int}")]
        public IActionResult Eliminar(int ID)
        {
            try
            {
                using (var conection = new SqlConnection(cadenaSQL))
                {
                    conection.Open();
                    var cmd = new SqlCommand("sp_DeleteLogger", conection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("Id", ID);
                    cmd.ExecuteNonQuery();
                }
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "User deleted" });
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });
            }
        }
    }
}
