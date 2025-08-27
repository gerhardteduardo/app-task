using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task.API.Data;
using Task.API.Models;

namespace Task.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTaskController : ControllerBase
    {
        // 1. Variável privada para guardar a instância do AppDbContext
        private readonly AppDbContext _context;

        // 2. Construtor que recebe o AppDbContext via Injeção de Dependência
        public UserTaskController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserTask>>> GetTasks()
        {
            return await _context.Tasks.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserTask>> GetTask(int id)
        {
            // Usa o FindAsync para buscar a tarefa no banco de dados
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                return NotFound(); // Retorna um erro 404
            }

            return task;
        }

        [HttpPost]
        public async Task<ActionResult<UserTask>> CreateTask([FromBody] UserTask newTask)
        {
            // Adiciona a nova tarefa ao contexto do banco de dados
            _context.Tasks.Add(newTask);

            // Salva as alterações no banco de dados de forma assíncrona
            await _context.SaveChangesAsync();

            // Retorna o status 201 Created com a URL do novo recurso
            return CreatedAtAction(nameof(GetTask), new { id = newTask.Id }, newTask);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, UserTask updatedTask)
        {
            // Verifica se o ID na URL corresponde ao ID no corpo da requisição
            if (id != updatedTask.Id)
            {
                return BadRequest(); // Retorna 400 Bad Request
            }

            // Marca o objeto como modificado no contexto.
            // O EF Core vai rastreá-lo e saber que ele precisa ser atualizado.
            _context.Entry(updatedTask).State = EntityState.Modified;

            try
            {
                // Salva as alterações no banco de dados de forma assíncrona
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Se a tarefa não existir, o EF Core dispara esta exceção
                if (!TaskExists(id))
                {
                    return NotFound(); // Retorna 404 Not Found
                }
                else
                {
                    // Se o erro for outro (concorrência, por exemplo), propaga a exceção
                    throw;
                }
            }

            // Retorna o status 204 No Content para uma atualização bem-sucedida
            return NoContent();
        }

        // Método auxiliar para verificar se uma tarefa existe
        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            // Busca a tarefa para deletar no banco de dados de forma assíncrona
            var taskToDelete = await _context.Tasks.FindAsync(id);

            if (taskToDelete == null)
            {
                return NotFound(); // Retorna 404 Not Found se a tarefa não existir
            }

            // Remove a tarefa do contexto
            _context.Tasks.Remove(taskToDelete);

            // Salva a remoção no banco de forma assíncrona
            await _context.SaveChangesAsync();

            return NoContent(); // Retorna 204 No Content para indicar sucesso
        }
    }
}
