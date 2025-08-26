using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Task.API.Models;

namespace Task.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTaskController : ControllerBase
    {
        // Uma lista estática para simular um banco de dados por enquanto.
        private static List<UserTask> _tasks = new List<UserTask>
        {
            new() { Id = 1, Name = "Comprar frutas",  State = true },
            new() { Id = 2, Name = "Malhar", State = true },
            new() { Id = 3, Name = "Arrumar cama", State = false }
        };

        [HttpGet]
        public ActionResult<IEnumerable<UserTask>> GetProdutos()
        {
            return _tasks;
        }

        [HttpGet("{id}")]
        public ActionResult<UserTask> GetTask(int id)
        {
            var task = _tasks.Find(t => t.Id == id);

            if (task == null)
            {
                return NotFound(); // Retorna um erro 404
            }

            return task;
        }

        [HttpPost]
        public ActionResult<UserTask> CreateTask([FromBody] UserTask newTask)
        {
            // Simula a adição de um novo ID
            int newId = _tasks.Max(t => t.Id) + 1;
            newTask.Id = newId;

            // Adiciona a nova tarefa na lista
            _tasks.Add(newTask);

            // Retorna o status 201 Created e a tarefa criada
            return CreatedAtAction(nameof(GetTask), new { id = newTask.Id }, newTask);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id, UserTask updatedTask)
        {
            if (id != updatedTask.Id)
            {
                return BadRequest(); // Retorna um erro 400 Bad Request se os IDs não coincidirem
            }

            var taskToUpdate = _tasks.Find(t => t.Id == id);

            if (taskToUpdate == null)
            {
                return NotFound(); // Retorna 404 Not Found se a tarefa não existir
            }

            taskToUpdate.Name = updatedTask.Name;
            taskToUpdate.State = updatedTask.State;

            return NoContent(); // Retorna 204 No Content, que é a resposta padrão para uma atualização bem-sucedida sem conteúdo para retornar
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTask(int id)
        {
            var taskToDelete = _tasks.Find(t => t.Id == id);

            if (taskToDelete == null)
            {
                return NotFound(); // Retorna 404 Not Found se a tarefa não existir
            }

            _tasks.Remove(taskToDelete);

            return NoContent(); // Retorna 204 No Content para indicar sucesso
        }
    }
}
