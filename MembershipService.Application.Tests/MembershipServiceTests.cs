using MembershipService.Application.Common.Interfaces;
using MembershipService.Application.DTOs; 
using MembershipService.Application.Services;
using MembershipService.Domain.Models; 
using MembershipService.Infrastructure.Integrations; 
using MembershipService.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MembershipService.Application.Tests
{
    [TestFixture] 
    public class MembershipInfoServiceTests
    {
        private Mock<ILogger<MembershipInfoService>> _mockLogger;
        private Mock<IVtexMembershipRepository> _mockVtexMembershipRepository;
        private MembershipInfoService _service;

        [SetUp] 
        public void Setup()
        {
            // 1. Create Mocks
            _mockLogger = new Mock<ILogger<MembershipInfoService>>();
            _mockVtexMembershipRepository = new Mock<IVtexMembershipRepository>();

            // 2. Instantiate the class under test
            _service = new MembershipInfoService(_mockLogger.Object, _mockVtexMembershipRepository.Object);
        }

        [Test] 
        public async Task GetActiveMembershipInfo_WhenRepositoryReturnsData_ShouldReturnMappedDtoList()
        {
            // Arrange
            var mockDomainData = new List<MembershipInfo> { new MembershipInfo { Id = "1" }, new MembershipInfo { Id = "2" } };

            _mockVtexMembershipRepository.Setup(repo => repo.GetActiveMembershipInfo()).ReturnsAsync(mockDomainData);

            // Act
            var result = await _service.GetActiveMembershipInfo();

            // Assert
            Assert.That(result, Is.Not.Null); 
            Assert.That(result.Count(), Is.EqualTo(2)); 
            Assert.That(result, Is.AssignableTo<IEnumerable<MembershipDto>>()); 

            var firstResult = result.First();
            Assert.That(firstResult.Id, Is.EqualTo(mockDomainData.First().Id)); 

            _mockVtexMembershipRepository.Verify(repo => repo.GetActiveMembershipInfo(), Times.Once);
        }

        [Test] 
        public async Task GetActiveMembershipInfo_WhenRepositoryReturnsNull_ShouldReturnEmptyList()
        {
            // Arrange
            _mockVtexMembershipRepository.Setup(repo => repo.GetActiveMembershipInfo()).ReturnsAsync((List<MembershipInfo>)null);

            // Act
            var result = await _service.GetActiveMembershipInfo();

            // Assert
            Assert.That(result, Is.Not.Null); 
            Assert.That(result, Is.Empty);
        }

        [Test] 
        public async Task GetActiveMembershipInfo_WhenRepositoryReturnsEmptyList_ShouldReturnEmptyList()
        {
            // Arrange
            _mockVtexMembershipRepository.Setup(repo => repo.GetActiveMembershipInfo()).ReturnsAsync(new List<MembershipInfo>());

            // Act
            var result = await _service.GetActiveMembershipInfo();

            // Assert
            Assert.That(result, Is.Not.Null); 
            Assert.That(result, Is.Empty);
        }
    }
}