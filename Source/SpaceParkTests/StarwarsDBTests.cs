﻿using SpacePark_ModelsDB.Database;
using SpacePark_ModelsDB.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using SpacePark_API.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SpaceParkTests
{
    public class StarwarsDBTests
    {
        private TestController _controller;
        private IStarwarsRepository _repository;

        public StarwarsDBTests()
        {
            _repository = GetInMemoryRepository();
            _controller = new TestController(_repository);
        }
        private void Populate(StarwarsContext context)
        {
            var xWing = new SpaceShip { Model = "X-Wing" };
            var luke = new Person() { Name = "Luke Skywalker" };
            var lsAccount = new Account() { SpaceShip = xWing, Person = luke };

            context.Add(lsAccount);

            context.SaveChanges();
        }
        private IStarwarsRepository GetInMemoryRepository()
        {
            var options = new DbContextOptionsBuilder<StarwarsContext>()
                             .UseInMemoryDatabase(databaseName: "MockDB")
                             .Options;

            var initContext = new StarwarsContext(options);
            initContext.Database.EnsureDeleted();

            Populate(initContext);
            var testContext = new StarwarsContext(options);
            var repository = new DbRepository(testContext);

            return repository;
        }
        [Fact]
        public void IndexPerson()
        {
            var result = _controller.Index() as ViewResult;
            var model = result.Model as IEnumerable<Person>;

            Assert.Single(model);
            Assert.Equal("Luke Skywalker", model.First().Name);
        }
    }
}
