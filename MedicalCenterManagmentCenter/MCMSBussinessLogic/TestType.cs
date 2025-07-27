 using MCMSDAL;
namespace MCMSBussinessLogic
{
        public class TestType
        {
            public TestTypeDto TDTO
            {
                get
                {
                    return new TestTypeDto
                    {
                        TestTypeId = TestTypeId,
                        Name = Name,
                        Description = Description,
                        Cost = Cost
                    };
                }
            }
            public int TestTypeId { get; set; }
            public string Name { get; set; }
            public string? Description { get; set; }
          
            public decimal Cost { get; set; }

            public TestType()
            {
                

                TestTypeId = 0;
                Name = "";
                Description = "";
                Cost = 0;
            }

            public TestType(TestTypeDto testTypeDto)
            {
                if (testTypeDto == null)
                    throw new ArgumentNullException(nameof(testTypeDto));

                TestTypeId = testTypeDto.TestTypeId;
                Name = testTypeDto.Name;
                Description = testTypeDto.Description;
                Cost = testTypeDto.Cost;
            }

            public async Task<bool> AddNewTestTypeAsync()
            {
                if (await IsTestTypeExistsByNameAsync(Name))
                {
                    return false;
                }

                if (Cost < 0)
                {
                    throw new InvalidOperationException("Cost cannot be negative");
                }

                int result = await MCMSDAL.TestTypeData.CreateTestType(TDTO);
                this.TestTypeId = result;
                return result > 0;
            }

            public static async Task<TestType?> FindTestTypeByIdAsync(int testTypeId)
            {
                var testTypeDto = await MCMSDAL.TestTypeData.GetTestTypeById(testTypeId);
                return (testTypeDto != null) ? new TestType(testTypeDto) : null;
            }

            public async Task<bool> UpdateTestTypeAsync()
            {
                if (!await MCMSDAL.TestTypeData.IsTestTypeExistsById(TestTypeId))
                    return false;

                 
               

                if (Cost < 0)
                {
                    throw new InvalidOperationException("Cost cannot be negative");
                }

                return await MCMSDAL.TestTypeData.UpdateTestType(TDTO);
            }

            public static async Task<bool> DeleteTestTypeByIdAsync(int testTypeId)
            {
                return await MCMSDAL.TestTypeData.DeleteTestType(testTypeId);
            }

            public static async Task<bool> IsTestTypeExistsByIdAsync(int testTypeId)
            {
                return await MCMSDAL.TestTypeData.IsTestTypeExistsById(testTypeId);
            }

            public static async Task<bool> IsTestTypeExistsByNameAsync(string name)
            {
            return await MCMSDAL.TestTypeData.IsTestTypeExistsByName(name);
            }

            public static async Task<TestType?> FindTestTypeByNameAsync(string name)
            {
                var allTestTypes = await MCMSDAL.TestTypeData.GetAllTestTypes();
                var testTypeDto = allTestTypes.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                return testTypeDto != null ? new TestType(testTypeDto) : null;
            }

        public static async Task<List<TestType>> GetAllTestTypesAsync()
        {
            var testTypeDtos = await MCMSDAL.TestTypeData.GetAllTestTypes();

           

            return testTypeDtos.Select(dto => new TestType(dto)).ToList();
        }

        public static async Task<List<TestType>> GetAllTestTypesAsync(int pageNumber = 1, int pageSize = 10)
            {
                var testTypeDtos = await MCMSDAL.TestTypeData.GetAllTestTypes();

                // Implement pagination
                var paginatedDtos = testTypeDtos
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return paginatedDtos.Select(dto => new TestType(dto)).ToList();
            }
        public static async Task<List<TestTypeSummaryDto>> GetTestTypeSummariesAsync()
        {
            var testTypes = await TestTypeData.GetAllTestTypes();

            return testTypes.Select(t => new TestTypeSummaryDto
            {
                TestTypeId = t.TestTypeId,
                Name = t.Name
            }).ToList();
        }


    }

}

