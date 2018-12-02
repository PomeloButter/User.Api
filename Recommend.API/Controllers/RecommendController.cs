using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recommend.API.Data;

namespace Recommend.API.Controllers
{
    [Route("api/recommends")]
    public class RecommendController : BaseController
    {
        private readonly RecommendContext _context;

        public RecommendController(RecommendContext context)
        {
            _context = context;
        }

        // GET api/values
        [HttpGet]
        [Route("")]
        public async  Task<IActionResult> Get()
        {
          return Ok(await _context.ProjectRecommends.AsNoTracking()
              .Where(p => p.UserId == UserIdentity.UserId)
              .ToListAsync());
        }
    }
}
