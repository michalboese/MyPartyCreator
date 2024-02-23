using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartyCreatorWebApi.Entities;
using PartyCreatorWebApi.Repositories;
using PartyCreatorWebApi.Repositories.Contracts;
using System;
using System.Threading.Tasks;

namespace PartyCreatorWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceiptItemController : ControllerBase
    {
        private readonly IReceiptItemRepository _receiptItemRepository;
        private readonly IUsersRepository _usersRepository;

        public ReceiptItemController(IReceiptItemRepository receiptItemRepository, IUsersRepository usersRepository)
        {
            _receiptItemRepository = receiptItemRepository;
            _usersRepository = usersRepository;
        }

        [HttpGet("GetReceiptItems/{eventId:int}"), Authorize]
        public async Task<ActionResult<List<ReceiptItem>>> GetReceiptItems(int eventId)
        {
            try
            {
            //trzeba zabezpieczyc 
            var receiptItems = await _receiptItemRepository.GetReceiptItems(eventId);
                return Ok(receiptItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

  

        [HttpPost("AddReceiptItem"), Authorize]
        public async Task<ActionResult<ReceiptItem>> AddReceiptItem([FromBody]ReceiptItem receiptItem)
        {
            try
            {
                var userId = Int32.Parse(_usersRepository.GetUserIdFromContext());
                var isOrganizer = await _receiptItemRepository.IsUserEventOrganizer(userId, receiptItem.EventId);

                if (!isOrganizer)
                {
                    return Forbid("Nie jesteś organizatorem tego wydarzenia");
                }

                var result = await _receiptItemRepository.AddReceiptItem(receiptItem);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }


       /* [HttpPut("UpdateReceiptItem"), Authorize]
        public async Task<ActionResult> UpdateReceiptItem(ReceiptItem receiptItem)
        {
            try
            {
                var result = await _receiptItemRepository.UpdateReceiptItem(receiptItem);

                if (result != null)
                    return Ok(result);
                else
                    return NotFound("Item not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        tez trzeba zwrocic przedmiot*/

        [HttpDelete("RemoveReceiptItem/{id:int}"), Authorize]
        public async Task<ActionResult<ReceiptItem>> RemoveReceiptItem(int id)
        {
            try
            {
               
                var itemToRemove = await _receiptItemRepository.GetReceiptItemById(id);

                if (itemToRemove == null)
                {
                    return NotFound("Item not found");
                }

                var userId = Int32.Parse(_usersRepository.GetUserIdFromContext());

                
                var isOrganizer = await _receiptItemRepository.IsUserEventOrganizer(userId, itemToRemove.EventId);

                if (!isOrganizer)
                {
                    return Forbid("Nie jesteś organizatorem tego wydarzenia");
                }

                var result = await _receiptItemRepository.RemoveReceiptItem(id);

                if (result != null )
                    return Ok(result);
                else
                    return NotFound("Item not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        
    }
}
