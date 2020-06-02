using AutoMapper;
using LibraryApi.Domain;
using LibraryApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;

namespace LibraryApi.Mappers
{
    public class EfBooksMapper : IMapBooks
    {
        LibraryDataContext Context;
        IMapper Mapper;
        MapperConfiguration Config;

        public EfBooksMapper(LibraryDataContext context, IMapper mapper, MapperConfiguration config)
        {
            Context = context;
            Mapper = mapper;
            Config = config;
        }

        public async Task<GetABookResponse> AddBook(PostBookCreate bookToAdd)
        {
            //var book = new Book
            //{
            //    Title = bookToAdd.Title,
            //    Author = bookToAdd.Author,
            //    Genre = bookToAdd.Genre,
            //    NumberOfPages = bookToAdd.NumberOfPages,
            //    InStock = true
            //};
            var book = Mapper.Map<Book>(bookToAdd);
            Context.Books.Add(book); // I have not Id!
            await Context.SaveChangesAsync(); // Suddenly I have an Id!
            // Book -> GetA BookResponse
            //var response = new GetABookResponse
            //{
            //    Id = book.Id,
            //    Title = book.Title,
            //    Author = book.Author,
            //    Genre = book.Genre,
            //    NumberOfPages = book.NumberOfPages
            //};
            var response = Mapper.Map<GetABookResponse>(book);
            return response;
        }

        public async Task<GetBooksResponse> GetAllBooksFor(string genre)
        {
            var books = Context.Books
                .Where(b => b.InStock)
                //.Select(book => Mapper.Map<GetBooksResponseItem>(book));
                .ProjectTo<GetBooksResponseItem>(Config)
                .AsNoTracking();


            if (genre != null)
            {
                books = books.Where(b => b.Genre == genre);
            }

            var booksList = await books.ToListAsync();
            var response = new GetBooksResponse
            {
                Books = booksList,
                GenreFilter = genre,
                NumberOfBooks = booksList.Count
            };
            return response;
        }
    }
}
