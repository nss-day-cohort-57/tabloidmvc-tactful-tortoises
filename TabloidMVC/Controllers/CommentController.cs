﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TabloidMVC.Repositories;
using TabloidMVC.Models;
using TabloidMVC.Models.ViewModels;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.VisualBasic;

namespace TabloidMVC.Controllers
{
    public class CommentController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICommentRepository _commentRepository;
        public CommentController(IPostRepository postRepository, ICategoryRepository categoryRepository, ICommentRepository commentRepository)
        {
            _postRepository = postRepository;
            _categoryRepository = categoryRepository;
            _commentRepository = commentRepository;
        }
        // GET: HomeController1
        public ActionResult Index(int id)
        {
            List<Comment> comments = _commentRepository.GetCommentsByPostId(id);
            var vm = new CommentPostViewModel();
            vm.PostId = id;
            vm.Comments = comments;

            return View(vm);
        }

        // GET: HomeController1/Details/5
        public ActionResult Details(int id)
        {
            List<Comment> comments = _commentRepository.GetCommentsByPostId(id);
            var vm = new CommentPostViewModel();
            vm.PostId = id;
            vm.Comments = comments;

            return View(vm);
        }

        // GET: HomeController1/Create
        public ActionResult Create(int id)
        {
            var vm = new CommentCreateViewModel();
            vm.PostId = id;
            return View(vm);
        }

        // POST: HomeController1/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CommentCreateViewModel vm, int id)
        {
            try
            {
                vm.Comment.UserProfileId = GetCurrentUserProfileId();
                vm.Comment.CreateDateTime = DateAndTime.Now;
                vm.Comment.PostId = id;

                _commentRepository.Add(vm.Comment);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: HomeController1/Edit/5
        public ActionResult Edit(int id)
        {
            var vm = new CommentCreateViewModel();
            vm.Comment = _commentRepository.GetCommentById(id);
            if(vm.Comment.UserProfileId != GetCurrentUserProfileId())
            {
                return NotFound();
            }
            return View(vm);
        }

        // POST: HomeController1/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: HomeController1/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: HomeController1/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        private int GetCurrentUserProfileId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(id);
        }
    }
}
