using Moq;
using SugarCounter.Api;
using SugarCounter.Api.Controllers.Food;
using SugarCounter.Core.Food;
using SugarCounter.Core.Users;

namespace Unit.Api.Controllers.Food
{
    public class FoodControllerTestsBase
    {
        public FoodControllerTestsBase()
        {
            UserContext = new RequestContext { CurrentUser = User };
            AdminContext = new RequestContext { CurrentUser = Admin };

            UserFood = new FoodItem() { Id = 1, UserInfoId = User.Id, Description = "food" };
            AnotherUserFood = new FoodItem() { Id = 2, UserInfoId = 99, Description = "other food" };

            FoodControllerForUser = new FoodController(FoodRepoMock.Object, NutritionProviderMock.Object, UserContext);
            FoodControllerForAdmin = new FoodController(FoodRepoMock.Object, NutritionProviderMock.Object, AdminContext);
        }

        protected UserInfo User { get; } = new UserInfo { Id = 1, Role = UserRole.User };
        protected UserInfo Admin { get; } = new UserInfo { Id = 2, Role = UserRole.Admin };

        protected RequestContext UserContext { get; }
        protected RequestContext AdminContext { get; }

        protected FoodItem UserFood { get; }
        protected FoodItem AnotherUserFood { get; }

        protected Mock<INutritionDataProvider> NutritionProviderMock { get; } = new Mock<INutritionDataProvider>();
        protected Mock<IFoodRepository> FoodRepoMock { get; } = new Mock<IFoodRepository>();
        protected FoodController FoodControllerForUser { get; }
        protected FoodController FoodControllerForAdmin { get; }
    }
}
