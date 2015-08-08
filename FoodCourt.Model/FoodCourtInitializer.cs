using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using FoodCourt.Model.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FoodCourt.Model
{
    public class FoodCourtInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            base.Seed(context);

            try
            {
                Group testGroup = new Group() {Name = "Test"};
                context.Groups.Add(testGroup);

                Poll testPoll = new Poll() {Group = testGroup};
                context.Polls.Add(testPoll);

                context.SaveChanges();

                SeedUsers(context, testGroup);

                Restaurant haLong = new Restaurant() {Group = testGroup, Name = "HaLong"};
                context.Restaurants.Add(haLong);

                Restaurant tommyBurger = new Restaurant() {Group = testGroup, Name = "Tommy Burger"};
                context.Restaurants.Add(tommyBurger);

                Restaurant daGrasso = new Restaurant() {Group = testGroup, Name = "DaGrasso"};
                context.Restaurants.Add(daGrasso);

                Restaurant uNotariusza = new Restaurant() {Group = testGroup, Name = "U'Notariusza"};
                context.Restaurants.Add(uNotariusza);


                Kind asian = new Kind() {Group = testGroup, Name = "Asian"};
                context.Kinds.Add(asian);

                Kind american = new Kind() {Group = testGroup, Name = "American"};
                context.Kinds.Add(american);

                Kind italian = new Kind() {Group = testGroup, Name = "Italian"};
                context.Kinds.Add(italian);

                Kind homemade = new Kind() {Group = testGroup, Name = "Homemade"};
                context.Kinds.Add(homemade);


                context.Dishes.Add(new Dish() {Restaurant = haLong, Kind = asian, Name = "Wołowina z grilla na ostro"});
                context.Dishes.Add(new Dish() {Restaurant = tommyBurger, Kind = american, Name = "Mariano Italiano"});
                context.Dishes.Add(new Dish() {Restaurant = daGrasso, Kind = italian, Name = "Polska"});
                context.Dishes.Add(new Dish() {Restaurant = uNotariusza, Kind = homemade, Name = "Naleśniki z serem"});

                context.SaveChanges();

                // randomly generate orders with above dishes
                foreach (ApplicationUser user in context.Users.ToList())
                {
                    int dishIndex = random.Next(4);

                    // each user has two random orders
                    context.Orders.Add(new Order()
                    {
                        Dish = context.Dishes.ToList().ElementAt(dishIndex),
                        CreatedBy = user,
                        Poll = testPoll
                    });

                    int newDishIndex;
                    do
                    {
                        newDishIndex = random.Next(4);
                    } while (newDishIndex == dishIndex);

                    // each user has two random orders
                    context.Orders.Add(new Order()
                    {
                        Dish = context.Dishes.ToList().ElementAt(newDishIndex),
                        CreatedBy = user,
                        Poll = testPoll
                    });
                }

                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void SeedUsers(ApplicationDbContext context, Group group)
        {
            UserManager<ApplicationUser, Guid> userManager = new UserManager<ApplicationUser, Guid>(new UserStore<ApplicationUser, Role, Guid, UserLogin, UserRole, UserClaim>(context));
            RoleManager<Role, Guid> roleManager = new RoleManager<Role, Guid>(new RoleStore<Role, Guid, UserRole>(context));
            
            for (int i = 0; i < 10; i++)
            {
                string randomString = RandomString(5);
                userManager.Create(new ApplicationUser { UserName = randomString, Email = randomString+"@example.com", EmailConfirmed = true, LockoutEnabled = true, Group = group }, "qwerty");
            }
        }
        
        private static Random random = new Random((int)DateTime.Now.Ticks);
        private string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }
    }
}
