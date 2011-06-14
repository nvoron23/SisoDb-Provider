﻿using System.Linq;
using SisoDb.Core;
using SisoDb.Providers.DbSchema;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;
using SisoDb.Structures.Schemas.MemberAccessors;

namespace SisoDb.Providers.Sql2008.DbSchema
{
    public class SqlDbIndexesSchemaBuilder : IDbSchemaBuilder
    {
        private readonly ISqlStatements _sqlStatements;
        private readonly SqlDbDataTypeTranslator _dataTypeTranslator;
        private readonly IDbColumnGenerator _columnGenerator;

        public SqlDbIndexesSchemaBuilder(ISqlStatements sqlStatements, IDbColumnGenerator columnGenerator)
        {
            _sqlStatements = sqlStatements.AssertNotNull("SqlStatements");
            _columnGenerator = columnGenerator.AssertNotNull("columnGenerator");
            _dataTypeTranslator = new SqlDbDataTypeTranslator();
        }

        public string GenerateSql(IStructureSchema structureSchema)
        {
            var columnDefinitions = structureSchema.IndexAccessors
                .Select(GenerateColumnDefinition);
            var columnsString = string.Join(",", columnDefinitions);
            var sql = structureSchema.IdAccessor.IdType == IdTypes.Guid
                          ? _sqlStatements.GetSql("CreateIndexesGuid")
                          : _sqlStatements.GetSql("CreateIndexesIdentity");

            return sql.Inject(
                structureSchema.GetIndexesTableName(),
                columnsString);
        }

        private string GenerateColumnDefinition(IIndexAccessor iac)
        {
            var dataTypeAsString = _dataTypeTranslator.ToDbType(iac);
            
            return _columnGenerator.ToSql(iac.Name, dataTypeAsString);
        }
    }
}