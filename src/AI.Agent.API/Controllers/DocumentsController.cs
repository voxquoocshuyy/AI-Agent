using AI.Agent.Domain.Entities;
using AI.Agent.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AI.Agent.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DocumentsController> _logger;

    public DocumentsController(
        IDocumentRepository documentRepository,
        IUnitOfWork unitOfWork,
        ILogger<DocumentsController> logger)
    {
        _documentRepository = documentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Gets all documents
    /// </summary>
    /// <returns>List of documents</returns>
    [HttpGet]
    [Authorize(Policy = "RequireUserRole")]
    [ProducesResponseType(typeof(IEnumerable<Document>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll()
    {
        var documents = await _documentRepository.GetAllAsync();
        return Ok(documents);
    }

    /// <summary>
    /// Gets a document by ID
    /// </summary>
    /// <param name="id">Document ID</param>
    /// <returns>Document details</returns>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = "RequireUserRole")]
    [ProducesResponseType(typeof(Document), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var document = await _documentRepository.GetByIdAsync(id);
        if (document == null)
        {
            return NotFound();
        }

        return Ok(document);
    }

    /// <summary>
    /// Creates a new document
    /// </summary>
    /// <param name="document">Document details</param>
    /// <returns>Created document</returns>
    [HttpPost]
    [Authorize(Policy = "RequireUserRole")]
    [ProducesResponseType(typeof(Document), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] Document document)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        document.CreatedAt = DateTime.UtcNow;
        document.IsProcessed = false;

        await _documentRepository.AddAsync(document);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Document created with ID: {DocumentId}", document.Id);

        return CreatedAtAction(nameof(GetById), new { id = document.Id }, document);
    }

    /// <summary>
    /// Updates an existing document
    /// </summary>
    /// <param name="id">Document ID</param>
    /// <param name="document">Updated document details</param>
    /// <returns>Updated document</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "RequireUserRole")]
    [ProducesResponseType(typeof(Document), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(Guid id, [FromBody] Document document)
    {
        if (!id.ToString().Equals(document.Id))
        {
            return BadRequest("ID mismatch");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingDocument = await _documentRepository.GetByIdAsync(id);
        if (existingDocument == null)
        {
            return NotFound();
        }

        existingDocument.Name = document.Name;
        existingDocument.Content = document.Content;
        existingDocument.FileType = document.FileType;
        existingDocument.LastModifiedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Document updated with ID: {DocumentId}", id);

        return Ok(existingDocument);
    }

    /// <summary>
    /// Deletes a document
    /// </summary>
    /// <param name="id">Document ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "RequireUserRole")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var document = await _documentRepository.GetByIdAsync(id);
        if (document == null)
        {
            return NotFound();
        }

        await _documentRepository.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Document deleted with ID: {DocumentId}", id);

        return NoContent();
    }
}