﻿using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using PineCone.Structures;
using SisoDb.Sql2008;
using SisoDb.Testing;

namespace SisoDb.Specifications.Sql2008.QueryEngine
{
    namespace GetByIdInterval
    {
        [Subject(typeof(Sql2008QueryEngine), "Get by Id interval")]
        public class when_getting_for_identities_and_no_items_exists : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create(StorageProviders.Sql2008);

            Because of = () =>
            {
                using (var qe = TestContext.Database.CreateQueryEngine())
                    _result = qe.GetByIdInterval<QueryIdentityItem>(1, 3).ToList();
            };

            It should_return_empty_result = () => _result.Count.ShouldEqual(0);

            private static IList<QueryIdentityItem> _result;
        }

        [Subject(typeof(Sql2008QueryEngine), "Get by Id interval")]
        public class when_getting_for_big_identities_and_no_items_exists : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create(StorageProviders.Sql2008);

            Because of = () =>
            {
                using (var qe = TestContext.Database.CreateQueryEngine())
                    _result = qe.GetByIdInterval<QueryBigIdentityItem>(1, 3).ToList();
            };

            It should_return_empty_result = () => _result.Count.ShouldEqual(0);

            private static IList<QueryBigIdentityItem> _result;
        }

        [Subject(typeof(Sql2008QueryEngine), "Get by Id interval")]
        public class when_getting_for_guids_and_no_items_exists : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
                _idFrom = SequentialGuid.New();
                _idTo = Enumerable.Repeat(SequentialGuid.New(), 10).Last();
            };

            Because of = () =>
            {
                using (var qe = TestContext.Database.CreateQueryEngine())
                    _result = qe.GetByIdInterval<QueryGuidItem>(_idFrom, _idTo).ToList();
            };

            It should_return_empty_result = () => _result.Count.ShouldEqual(0);

            private static Guid _idFrom, _idTo;
            private static IList<QueryGuidItem> _result;
        }

        [Subject(typeof(Sql2008QueryEngine), "Get by Id interval")]
        public class when_getting_for_identities_and_range_matches_subset_of_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
                using (var uow = TestContext.Database.CreateUnitOfWork())
                {
                    uow.InsertMany(new[]
                    {
                        new QueryIdentityItem{SortOrder = 1, StringValue = "A"},
                        new QueryIdentityItem{SortOrder = 2, StringValue = "B"},
                        new QueryIdentityItem{SortOrder = 3, StringValue = "C"},
                        new QueryIdentityItem{SortOrder = 4, StringValue = "D"},
                    });

                    uow.Commit();
                }
            };

            Because of = () =>
            {
                using (var qe = TestContext.Database.CreateQueryEngine())
                    _result = qe.GetByIdInterval<QueryIdentityItem>(2, 3).ToList();
            };

            It should_have_subset_count_of_2 = () => _result.Count.ShouldEqual(2);

            It should_have_subset_containing_the_two_middle_items = () =>
            {
                _result.First().SortOrder.ShouldEqual(2);
                _result.Last().SortOrder.ShouldEqual(3);
            };

            private static IList<QueryIdentityItem> _result;
        }

        [Subject(typeof(Sql2008QueryEngine), "Get by Id interval")]
        public class when_getting_for_big_identities_and_range_matches_subset_of_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
                using (var uow = TestContext.Database.CreateUnitOfWork())
                {
                    uow.InsertMany(new[]
                    {
                        new QueryBigIdentityItem{SortOrder = 1, StringValue = "A"},
                        new QueryBigIdentityItem{SortOrder = 2, StringValue = "B"},
                        new QueryBigIdentityItem{SortOrder = 3, StringValue = "C"},
                        new QueryBigIdentityItem{SortOrder = 4, StringValue = "D"},
                    });

                    uow.Commit();
                }
            };

            Because of = () =>
            {
                using (var qe = TestContext.Database.CreateQueryEngine())
                    _result = qe.GetByIdInterval<QueryBigIdentityItem>(2, 3).ToList();
            };

            It should_have_subset_count_of_2 = () => _result.Count.ShouldEqual(2);

            It should_have_subset_containing_the_two_middle_items = () =>
            {
                _result.First().SortOrder.ShouldEqual(2);
                _result.Last().SortOrder.ShouldEqual(3);
            };

            private static IList<QueryBigIdentityItem> _result;
        }

        [Subject(typeof(Sql2008QueryEngine), "Get by Id interval")]
        public class when_getting_for_guids_and_range_matches_subset_of_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);

                var items = new[]
                    {
                        new QueryGuidItem{SortOrder = 1, StringValue = "A"},
                        new QueryGuidItem{SortOrder = 2, StringValue = "B"},
                        new QueryGuidItem{SortOrder = 3, StringValue = "C"},
                        new QueryGuidItem{SortOrder = 4, StringValue = "D"},
                    };

                using (var uow = TestContext.Database.CreateUnitOfWork())
                {
                    uow.InsertMany(items);
                    uow.Commit();
                }

                _idFrom = items[1].StructureId;
                _idTo = items[2].StructureId;
            };

            Because of = () =>
            {
                using (var qe = TestContext.Database.CreateQueryEngine())
                    _result = qe.GetByIdInterval<QueryGuidItem>(_idFrom, _idTo).ToList();
            };

            It should_have_subset_count_of_2 = () => _result.Count.ShouldEqual(2);

            It should_have_subset_containing_the_two_middle_items = () =>
            {
                _result.First().SortOrder.ShouldEqual(2);
                _result.Last().SortOrder.ShouldEqual(3);
            };

            private static Guid _idFrom, _idTo;
            private static IList<QueryGuidItem> _result;
        }
    }
}