using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using FoodCourt.Controllers.Base;
using FoodCourt.Model;
using FoodCourt.Service;
using FoodCourt.ViewModel;

namespace FoodCourt.Controllers
{
    public class KindController : BaseApiController
    {
        public async Task<IHttpActionResult> GetList()
        {
            return await Search("");
        }

        public async Task<IHttpActionResult> Search(string searchPhrase)
        {
            var query = UnitOfWork.KindRepository.Search(searchPhrase);
            var viewModelQuery = query.Select(k => new KindViewModel()
            {
                Id = k.Id,
                Name = k.Name
            });

            var viewModelList = await viewModelQuery.ToListAsync();

            return Ok(viewModelList);
        }

        public async Task<IHttpActionResult> Put(KindViewModel kind)
        {
            Kind newKind = new Kind()
            {
                Name = kind.Name
            };

            await UnitOfWork.KindRepository.Insert(newKind);

            return Ok(new KindViewModel()
            {
                Id = newKind.Id,
                Name = newKind.Name
            });
        }

        public async Task<IHttpActionResult> Post(KindViewModel kind)
        {
            throw new NotImplementedException();
        }
    }
}