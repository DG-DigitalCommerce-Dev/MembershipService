using AutoMapper;
using MembershipService.Application.DTOs;
using MembershipService.Application.Services;
using MembershipService.Domain.Models;
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
    public class MembershipDataServiceTests
    {
        private Mock<ILogger<MembershipDataService>> _mockLogger;
        private Mock<IVtexMembershipRepository> _mockVtexMembershipRepository;
        private Mock<IMapper> _mockMapper;
        private MembershipDataService _service;

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<MembershipDataService>>();
            _mockVtexMembershipRepository = new Mock<IVtexMembershipRepository>();
            _mockMapper = new Mock<IMapper>();
            _service = new MembershipDataService(_mockLogger.Object,_mockVtexMembershipRepository.Object,_mockMapper.Object);
        }

        [Test]
        public async Task GetActiveMembershipData_WhenRepositoryReturnsNull_ReturnsNull()
        {
            // Arrange
            int page = 1;
            _mockVtexMembershipRepository.Setup(repo => repo.GetActiveMembershipData(page)).ReturnsAsync((VtexMembershipResponse)null);

            // Act
            var result = await _service.GetActiveMembershipData(page);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetActiveMembershipData_WhenRepositoryReturnsData_ReturnsMappedDto()
        {
            // Arrange
            int page = 1;
            int totalCount = 2;

            var membershipDataList = new List<MembershipData>
            {
                new MembershipData { Id = "123", CustomerEmail = "test1@domain.com" },
                new MembershipData { Id = "123", CustomerEmail = "test2@domain.com" }
            };
            var repoResult = new VtexMembershipResponse(membershipDataList, totalCount);

            var mappedDtos = new List<MembershipDto>
            {
                new MembershipDto { Id = membershipDataList[0].Id, CustomerEmail = membershipDataList[0].CustomerEmail },
                new MembershipDto { Id = membershipDataList[1].Id, CustomerEmail = membershipDataList[1].CustomerEmail}
            };

            _mockVtexMembershipRepository.Setup(repo => repo.GetActiveMembershipData(page)).ReturnsAsync(repoResult);

            _mockMapper.Setup(m => m.Map<IEnumerable<MembershipDto>>(membershipDataList)).Returns(mappedDtos);

            // Act
            var result = await _service.GetActiveMembershipData(page);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<MembershipResponseDto>());
            Assert.Multiple(() =>
            {
                Assert.That(result.TotalCount, Is.EqualTo(totalCount));
                Assert.That(result.MembershipList, Is.SameAs(mappedDtos)); 
                Assert.That(result.MembershipList.Count(), Is.EqualTo(2));
            });

            _mockVtexMembershipRepository.Verify(repo => repo.GetActiveMembershipData(page), Times.Once);
            _mockMapper.Verify(m => m.Map<IEnumerable<MembershipDto>>(membershipDataList), Times.Once);
        }

        [Test]
        public async Task GetActiveMembershipData_WhenRepositoryReturnsEmptyList_ReturnsDtoWithEmptyList()
        {
            // Arrange
            int page = 1;
            int totalCount = 0;
            var membershipDataList = new List<MembershipData>();
            var repoResult = new VtexMembershipResponse(membershipDataList,totalCount);
            var membershipDtoList = new List<MembershipDto>();

            _mockVtexMembershipRepository.Setup(repo => repo.GetActiveMembershipData(page)).ReturnsAsync(repoResult);
            _mockMapper.Setup(m => m.Map<IEnumerable<MembershipDto>>(membershipDataList)).Returns(membershipDtoList);

            // Act
            var result = await _service.GetActiveMembershipData(page);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.TotalCount, Is.EqualTo(0));
                Assert.That(result.MembershipList, Is.Not.Null);
                Assert.That(result.MembershipList.Any(), Is.False);
                Assert.That(result.MembershipList, Is.SameAs(membershipDtoList));
            });
        }
    }



}
