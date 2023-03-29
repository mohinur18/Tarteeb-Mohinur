﻿//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using Tarteeb.Api.Models.Foundations.Scores;
using Tarteeb.Api.Models.Foundations.Scores.Exceptionis;
using Xunit;

namespace Tarteeb.Api.Tests.Unit.Services.Foundations.Scores
{
    public partial class ScoreServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedScoreStorageException =
                new FailedScoreStorageException(sqlException);

            var expectedScoreDependencyException =
                new ScoreDependencyException(failedScoreStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectScoreByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Score> retrieveScoreByIdTask = 
                this.scoreService.RetrieveScoreByIdAsync(someId);

            ScoreDependencyException actualScoreDependencyException =
                await Assert.ThrowsAsync<ScoreDependencyException>(
                    retrieveScoreByIdTask.AsTask);

            // then 
            actualScoreDependencyException.Should().BeEquivalentTo(
                expectedScoreDependencyException);

            this.storageBrokerMock.Verify(broker =>
            broker.SelectScoreByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
