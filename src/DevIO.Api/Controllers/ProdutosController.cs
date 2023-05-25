﻿using AutoMapper;
using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DevIO.Api.Controllers
{
    [Route("api/produtos")]
    public class ProdutosController : MainController
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IProdutoService _produtoService;
        private readonly IMapper _mapper;


        public ProdutosController(INotificador notificador,
                                  IProdutoService produtoService,
                                  IProdutoRepository produtoRepository, 
                                  IMapper mapper) : base(notificador)
        {
            _produtoService = produtoService;
            _produtoRepository = produtoRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<ProdutoViewModel>> ObterTodos()
        {

            return _mapper.Map<IEnumerable<ProdutoViewModel>>(await _produtoRepository.ObterProdutosFornecedores());

        }
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProdutoViewModel>> ObterPorId(Guid id)
        {

            var produtoViewModel = await ObterProduto(id);

            if (produtoViewModel == null) return NotFound();

            return produtoViewModel;
        }
        [HttpPost]
        public async Task<ActionResult<ProdutoViewModel>> Adicionar(ProdutoViewModel produtoViewModel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var imagemNome = Guid.NewGuid() + "_" + produtoViewModel.Imagem;
            if(!UploadArquivo(produtoViewModel.ImagemUpload, imagemNome))
            {
                return CustomResponse(produtoViewModel);
            }
            produtoViewModel.Imagem = imagemNome;

            await _produtoService.Adicionar(_mapper.Map<Produto>(produtoViewModel));

            return CustomResponse(produtoViewModel);
        }

        [HttpPost("Adicionar")]
        public async Task<ActionResult<ProdutoViewModel>> AdicionarAlternativo(ProdutoImagemViewModel produtoViewModel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var imgPrefixo = Guid.NewGuid() + "_";
            if (! await UploadArquivoAlternativo(produtoViewModel.ImagemUpload, imgPrefixo))
            {
                return CustomResponse(ModelState);
            }
            produtoViewModel.Imagem = imgPrefixo + produtoViewModel.ImagemUpload;

            await _produtoService.Adicionar(_mapper.Map<Produto>(produtoViewModel));

            return CustomResponse(produtoViewModel);
        }
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ProdutoViewModel>> Atualizar(Guid id, ProdutoViewModel produtoViewModel)
        {

            if (id != produtoViewModel.Id) return NotFound();

            var produtoAtualizacao = await ObterProduto(id);
            produtoViewModel.Imagem = produtoAtualizacao.Imagem;
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            if(produtoViewModel.ImagemUpload != null)
            {
                var imagemNome = Guid.NewGuid() + "_" + produtoViewModel.Imagem;
                if (!UploadArquivo(produtoViewModel.ImagemUpload, imagemNome))
                {
                    return CustomResponse(ModelState);
                }
                produtoAtualizacao.Imagem = imagemNome;
            }
            produtoAtualizacao.Nome = produtoViewModel.Nome;
            produtoAtualizacao.Descricao = produtoViewModel.Descricao;
            produtoAtualizacao.Valor = produtoViewModel.Valor;
            produtoAtualizacao.Ativo = produtoViewModel.Ativo;

            await _produtoService.Atualizar(_mapper.Map<Produto>(produtoAtualizacao));
            return CustomResponse(produtoViewModel);
        }



        [DisableRequestSizeLimit]
        [HttpPost("Imagem")]
        public async Task<ActionResult> AdicionarImagem(IFormFile file)
        {
            return Ok(file);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ProdutoViewModel>> Excluir(Guid id)
        {
            var produto = await ObterProduto(id);
            if (produto == null) return NotFound();

            await _produtoService.Remover(id);
            return CustomResponse(produto);
        }

        private  bool UploadArquivo(string arquivo, string imgNome)
        {
            

            if (string.IsNullOrEmpty(arquivo))
            {
                NotificarErro(mensagem: "Forneça uma imagem para esse produto!");
                return false;
            }
            var imageDataByteArray = Convert.FromBase64String(arquivo);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", imgNome);

            if (System.IO.File.Exists(filePath))
            {
                NotificarErro(mensagem: "Já existe um arquivo com este nome!");
                return false;
            }

            System.IO.File.WriteAllBytes(filePath, imageDataByteArray);
  
            return true;
        }

        private async Task<bool> UploadArquivoAlternativo(IFormFile arquivo, string imgPrefixo)
        {


            if (arquivo == null || arquivo.Length ==0)
            {
                NotificarErro(mensagem: "Forneça uma imagem para esse produto!");
                return false;
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", imgPrefixo + arquivo.FileName);

            if (System.IO.File.Exists(path))
            {
                NotificarErro(mensagem: "Já existe um arquivo com este nome!");
                return false;
            }

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            return true;
        }


        private async Task<ProdutoViewModel> ObterProduto(Guid id)
        {

            return _mapper.Map<ProdutoViewModel>(await _produtoRepository.ObterProdutoFornecedor());
            

        }

    }
}
