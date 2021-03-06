﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2049
{
	using System.Threading.Tasks;
	[TestFixture]
	public class Fixture2049Async : BugTestCase
	{
		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (ISession session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var p = new Person {Id = 1, Name = "Name"};
				session.Save(p);
				var ic = new IndividualCustomer {Deleted = false, Person = p, Id = 1};
				session.Save(ic);

				var deletedPerson = new Person {Id = 2, Name = "Name Deleted"};
				session.Save(deletedPerson);
				var deletedCustomer = new IndividualCustomer {Deleted = true, Person = deletedPerson, Id = 2};
				session.Save(deletedCustomer);

				tx.Commit();
			}
		}


		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (ISession session = OpenSession())
			{
				session.Delete("from System.Object");
				session.Flush();
			}
		}


		[Test]
		[KnownBug("Known bug NH-2049.")]
		public async Task CanCriteriaQueryWithFilterOnJoinClassBaseClassPropertyAsync()
		{
			using (ISession session = OpenSession())
			{
				session.EnableFilter("DeletedCustomer").SetParameter("deleted", false);
				IList<Person> persons = await (session.CreateCriteria(typeof (Person)).ListAsync<Person>());

				Assert.That(persons, Has.Count.EqualTo(1));
				Assert.That(persons[0].Id, Is.EqualTo(1));
				Assert.That(persons[0].IndividualCustomer, Is.Not.Null);
				Assert.That(persons[0].IndividualCustomer.Id, Is.EqualTo(1));
				Assert.That(persons[0].IndividualCustomer.Deleted, Is.False);
			}
		}


		[Test]
		[KnownBug("Known bug NH-2049.", "NHibernate.Exceptions.GenericADOException")]
		public async Task CanHqlQueryWithFilterOnJoinClassBaseClassPropertyAsync()
		{
			using (ISession session = OpenSession())
			{
				session.EnableFilter("DeletedCustomer").SetParameter("deleted", false);
				var persons = await (session.CreateQuery("from Person as person left join person.IndividualCustomer as indCustomer")
					.ListAsync<Person>());

				Assert.That(persons, Has.Count.EqualTo(1));
				Assert.That(persons[0].Id, Is.EqualTo(1));
				Assert.That(persons[0].IndividualCustomer, Is.Not.Null);
				Assert.That(persons[0].IndividualCustomer.Id, Is.EqualTo(1));
				Assert.That(persons[0].IndividualCustomer.Deleted, Is.False);
			}
		}
	}
}
