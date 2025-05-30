using APIFuncional.Data;
using APIFuncional.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIFuncional.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/produtos")]
    public class ProtudosController : ControllerBase
    {
        private readonly ApiDbContext _context;
        public ProtudosController(ApiDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType] //É uma forma de dar um retorno para algum code que não estamos declarando
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos()
        {
            if (_context.Produtos == null) return NotFound();

            return await _context.Produtos.ToListAsync();
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Produto>> GetProduto(int id)
        {
            if (_context.Produtos == null) return NotFound(); //Verificar se a tabela não tem registro

            var produto = await _context.Produtos.FindAsync(id);

            if (produto == null) return NotFound(); //Verificar se o Id informado não tem registro

            return produto;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Produto>> PostProduto(Produto produto)
        {
            if (_context.Produtos == null)
            {
                return Problem("Erro ao criar um Produto, contate o suporte!");
            }

            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);

                //return ValidationProblem(ModelState); //Recomendado

                return ValidationProblem(new ValidationProblemDetails(ModelState)
                {
                    Title = "Um ou mais erros de validação ocorreram!"
                });
            }

            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProdutos), new { id = produto.Id }, produto);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> PutProduto(int id, Produto produto)
        {
            if (id != produto.Id) return BadRequest();
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            //_context.Produtos.Update(produto);

            _context.Entry(produto).State = EntityState.Modified; //Deixa o estado do produto como "modificado", diferente do update
                                                                  //que tenta ataxa o produto na memoria e não leva em consideração  
            try                                                   //o produto que já está ataxado.
            {
                await _context.SaveChangesAsync(); //Faz a alteração
            }
            catch (DbUpdateConcurrencyException) //Caso não de certo vai ser um problema de concorrencia onde dois ou mais
            {                                    //usuários ou mais tentam fazer uma solicitação ao mesmo tempo
                if (!ProdutoExistis(id))
                {
                    return NotFound();
                }

                throw;
            }//não é recomendado fazer sempre.

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> DeleteProduto(int id)
        {
            if(_context.Produtos == null) return NotFound(); //Verificar se a tabela não tem registro

            var produto = await _context.Produtos.FindAsync(id);

            if (produto == null) return NotFound(); //Verifica se o produto não existe

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProdutoExistis(int id) //Metodo para verificar se o produto existe
        {
            return (_context.Produtos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
