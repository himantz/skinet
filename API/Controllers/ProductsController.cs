using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Errors;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specification;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers {
  
    public class ProductsController : BaseApiController
    {

        private readonly IGenericRepository<Product> _ProductsRepo;
        private readonly IGenericRepository<ProductBrand> _ProductBrandRepo;
        private readonly IGenericRepository<ProductType> _ProductTypeRepo;
        private readonly IMapper _mapper;

        public ProductsController (IGenericRepository<Product> ProductsRepo,
                                   IGenericRepository<ProductBrand> ProductBrandRepo,
                                   IGenericRepository<ProductType> ProductTypeRepo,
                                   IMapper mapper) {
            _ProductsRepo = ProductsRepo;
            _ProductTypeRepo = ProductTypeRepo;
            _mapper = mapper;
            _ProductBrandRepo = ProductBrandRepo;

        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProducts () 
        {
            var spec = new ProductsWithTypesAndBrandsSpecification();

            var products = await _ProductsRepo.ListAsync(spec);

            return Ok(_mapper.Map<IReadOnlyList<Product>,IReadOnlyList<ProductToReturnDto>>(products));
        }

        [HttpGet ("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct (int id) 
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(id);

            var product = await _ProductsRepo.GetEntityWithSpec(spec);

            if(product == null)
            {
                return NotFound(new ApiResponse(404));
            }

            return _mapper.Map<Product, ProductToReturnDto>(product);
        }

        [HttpGet ("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProdctBrands () 
        {
            return Ok (await _ProductBrandRepo.ListAllAsync());
        }

        [HttpGet ("types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProdctTypes () 
        {
            return Ok (await _ProductTypeRepo.ListAllAsync());
        }
    }
}