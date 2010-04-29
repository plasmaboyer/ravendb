using System.ComponentModel.Composition.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Raven.Database;
using Raven.Database.Exceptions;
using Raven.Tests.Storage;
using Xunit;
using Raven.Database.Json;

namespace Raven.Tests.Triggers
{
	public class PutTriggers : AbstractDocumentStorageTest
	{
		private readonly DocumentDatabase db;

		public PutTriggers()
		{
			db = new DocumentDatabase(new RavenConfiguration
			{
				DataDirectory = "raven.db.test.esent",
				Container = new CompositionContainer(new TypeCatalog(
					typeof(VetoCapitalNamesPutTrigger),
					typeof(AuditPutTrigger)))
			});
		}

		public override void Dispose()
		{
			db.Dispose();
			base.Dispose();
		}

		[Fact]
		public void CanPutDocumentWithLowerCaseName()
		{
			db.Put("abc", null, JObject.Parse("{'name': 'abc'}"), new JObject(), null);

			Assert.Contains("\"name\":\"abc\"", db.Get("abc", null).ToJson().ToString(Formatting.None));
		}

		[Fact]
		public void TriggerCanModifyDocumentBeforeInsert()
		{
			db.Put("abc", null, JObject.Parse("{'name': 'abc'}"), new JObject(), null);

			var actualString = db.Get("abc", null).Data.ToJObject().ToString(Formatting.None);
			Assert.Contains(@"{""name"":""abc"",""created_at"":""\/Date(946677600000+0200)\/""}", actualString);
		}

		[Fact]
		public void CannotPutDocumentWithUpperCaseNames()
		{
			Assert.Throws<OperationVetoedException>(
				() => db.Put("abc", null, JObject.Parse("{'name': 'ABC'}"), new JObject(), null));
		}
	}
}