using ContactsAPI.Data;
using ContactsAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContactsAPI.Controllers
{
    [ApiController]
    // this will be same as [Route("api/contacts")]
    [Route("api/[controller]")]
    public class ContactsController : Controller
    {
        private readonly ContactsAPIDbContext dbContext;
        public ContactsController(ContactsAPIDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        //The name of this method is GetContacts and this is returning an IActionResult
        [HttpGet] 
        public async Task<IActionResult> GetContacts()
        {
           return Ok( await dbContext.Contacts.ToListAsync());
            
        }
        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetContact([FromRoute] Guid id)
        {
            var contact = await dbContext.Contacts.FindAsync(id);
            if(contact == null)
            {
                return NotFound();
            }
            return Ok(contact);
        }

        //First create a class and name it AddContactRequest and we do this we dont want to use Id and give 
        //our own Id so 
        [HttpPost]
        public async Task<IActionResult> AddContacts(AddContactRequest addContactRequest)
        {
            //Below we are mapping between  AddContactRequest and  Contact model Now That is what goes
            //Into our database
            var contact = new Contact()
            {
                Id = Guid.NewGuid(),
                Address = addContactRequest.Address,
                Email = addContactRequest.Email,
                FullName = addContactRequest.FullName,
                Phone = addContactRequest.Phone,
            };
            //Now we  can talk to our  (dbContext)database using AddContactRequest and store this new Contacts
            await dbContext.Contacts.AddAsync(contact);
            //with entity framework core we have to save if before we actually see it
            await dbContext.SaveChangesAsync();
            //now its time to retern the request we have created(contact)
            //wrapped inside an Ok ressponse
            return Ok(contact);
            

        }

        //It's an update so we use PUT
        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateContact([FromRoute] Guid id,UpdateContactRequest updateContactRequest)
        {
           var contact =  await dbContext.Contacts.FindAsync(id);
            if(contact != null)
            {
                contact.FullName = updateContactRequest.FullName;
                contact.Phone = updateContactRequest.Phone;
                contact.Email = updateContactRequest.Email;
                contact.Address = updateContactRequest.Address;
               await dbContext.SaveChangesAsync();
                return Ok(contact);     
            }
            return NotFound();

        }
        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteContact([FromRoute] Guid id)
        {
             var contact = await dbContext.Contacts.FindAsync(id);
            if( contact != null)
            {
                dbContext.Remove(contact);
               await dbContext.SaveChangesAsync();
                return Ok(contact);

            }
            return NotFound();
        }

    }
}
