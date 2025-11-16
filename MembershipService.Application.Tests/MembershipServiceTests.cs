using AutoMapper;
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
        private Mock<ILogger<MembershipDataService>> _mockLogger;
        private Mock<IVtexMembershipRepository> _mockVtexMembershipRepository;
        private Mock<IMapper> _mapper;
        private MembershipDataService _service;

        [SetUp] 
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<MembershipDataService>>();
            _mockVtexMembershipRepository = new Mock<IVtexMembershipRepository>();
            _mapper = new Mock<IMapper>();

            _service = new MembershipDataService(_mockLogger.Object, _mockVtexMembershipRepository.Object,_mapper.Object);
        }

        [Test] 
        public async Task GetActiveMembershipInfo_WhenRepositoryReturnsData_ShouldReturnMappedDtoList()
        {
            // Arrange
            var mockDomainData = new List<MembershipData> { new MembershipData { Id = "1" }};

            _mockVtexMembershipRepository.Setup(repo => repo.GetActiveMembershipData()).ReturnsAsync(mockDomainData);
            _mapper.Setup(mapper => mapper.Map<IEnumerable<MembershipDto>>(It.IsAny<IEnumerable<MembershipData>>()))
                .Returns((IEnumerable<MembershipData> data) => new List<MembershipDto> { new MembershipDto { Id = data.First().Id } });
            // Act
            var result = await _service.GetActiveMembershipData();

            // Assert
            Assert.That(result, Is.Not.Null); 
            Assert.That(result.Count(), Is.EqualTo(1)); 
            Assert.That(result, Is.AssignableTo<IEnumerable<MembershipDto>>()); 

            var firstResult = result.First();
            Assert.That(firstResult.Id, Is.EqualTo(mockDomainData.First().Id)); 

            _mockVtexMembershipRepository.Verify(repo => repo.GetActiveMembershipData(), Times.Once);
        }

        [Test] 
        public async Task GetActiveMembershipInfo_WhenRepositoryReturnsNull_ShouldReturnEmptyList()
        {
            // Arrange
            _mockVtexMembershipRepository.Setup(repo => repo.GetActiveMembershipData()).ReturnsAsync((List<MembershipData>)null);

            // Act
            var result = await _service.GetActiveMembershipData();

            // Assert
            Assert.That(result, Is.Not.Null); 
            Assert.That(result, Is.Empty);
        }

        [Test] 
        public async Task GetActiveMembershipInfo_WhenRepositoryReturnsEmptyList_ShouldReturnEmptyList()
        {
            // Arrange
            _mockVtexMembershipRepository.Setup(repo => repo.GetActiveMembershipData()).ReturnsAsync(new List<MembershipData>());

            // Act
            var result = await _service.GetActiveMembershipData();

            // Assert
            Assert.That(result, Is.Not.Null); 
            Assert.That(result, Is.Empty);
        }
    }
}