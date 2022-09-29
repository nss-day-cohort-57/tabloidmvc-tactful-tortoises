using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using System.Security.Claims;
using TabloidMVC.Models.ViewModels;
using TabloidMVC.Repositories;
using TabloidMVC.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace TabloidMVC.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IUserProfileRepository _profileRepository;
        public PostController(IPostRepository postRepository, ICategoryRepository categoryRepository, ICommentRepository commentRepository, IUserProfileRepository profileRepository)
        {
            _postRepository = postRepository;
            _categoryRepository = categoryRepository;
            _commentRepository = commentRepository;
            _profileRepository = profileRepository;
        }

        public IActionResult Index()
        {
            var posts = _postRepository.GetAllPublishedPosts();
            return View(posts);
        }

        public IActionResult UserIndex()
        {
            int userId = GetCurrentUserProfileId();
            var posts = _postRepository.GetPostsByUserId(userId);
            return View(posts);
        }

        public IActionResult PendingPosts()
        {
            var vm = new PendingPostViewModel()
            {
                Posts = _postRepository.GetPendingPosts()
            };
           return View(vm);
        }

        public IActionResult ApprovePost(int id)
        {
            var vm = new PostApprovalViewModel()
            {
                Post = _postRepository.GetPostById(id)
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApprovePost(PostApprovalViewModel vm, int id)
        {
            try
            {
               
                vm.Post.IsApproved = true;

                _postRepository.UpdatePost(vm.Post);
                return RedirectToAction("PendingPosts");
            }
            catch
            {
               
                return View(vm);
            }
        }
        public IActionResult Details(int id)
        {
            var post = _postRepository.GetPublishedPostById(id);
            if (post == null)
            {
                int userId = GetCurrentUserProfileId();
                post = _postRepository.GetUserPostById(id, userId);
                if (post == null)
                {
                    return NotFound();
                }
            }
            return View(post);
        }

       

        public IActionResult Create()
        {
            var vm = new PostCreateViewModel();
            vm.CategoryOptions = _categoryRepository.GetAll();
            return View(vm);
        }

        [HttpPost]
        public IActionResult Create(PostCreateViewModel vm)
        {
            try
            {
                vm.Post.CreateDateTime = DateAndTime.Now;
                vm.Post.UserProfileId = GetCurrentUserProfileId();
                UserProfile profile = _profileRepository.GetById(vm.Post.UserProfileId);
                if (profile.UserType.Name == "Admin")
                {
                    vm.Post.IsApproved = true;
                }
                else
                {
                    vm.Post.IsApproved = false;
                }

                _postRepository.Add(vm.Post);

                return RedirectToAction("Details", new { id = vm.Post.Id });
            } 
            catch
            {
                vm.CategoryOptions = _categoryRepository.GetAll();
                return View(vm);
            }
        }

        public IActionResult Edit(int id)
        {
            var vm = new PostCreateViewModel();
            vm.Post = _postRepository.GetPublishedPostById(id);
            vm.CategoryOptions = _categoryRepository.GetAll().ToList();
            
            if(vm.Post == null)
            {
                return NotFound();
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(PostCreateViewModel vm, int id)
        {
            try 
            {
                vm.Post.Id = id;
                vm.Post.UserProfileId = GetCurrentUserProfileId();
               
                _postRepository.UpdatePost(vm.Post);
                return RedirectToAction("Details", new { id = vm.Post.Id });
            }
            catch
            {
                vm.CategoryOptions = _categoryRepository.GetAll();
                return View(vm);
            }
        }

        public IActionResult Delete(int id)
        {
            var vm = new PostDeleteViewModel();
            vm.Post = _postRepository.GetPostById(id);
            if(vm.Post.UserProfileId == GetCurrentUserProfileId())
            {
                return View(vm);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(PostDeleteViewModel vm, int id)
        {
            try
            {
                _postRepository.DeletePost(id);
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
