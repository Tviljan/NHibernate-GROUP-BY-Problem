# NHibernate-GROUP-BY-Problem

QueryOver query does not work. It creates SQL query as follows:

SELECT this_.ID as ID0_0_, this_.Source as Source0_0_, this_.Target as Target0_0_, this_.SendDateTime as SendDate4_0_0_, this_.Version as Version0_0_ FROM [DataItem] this_ 
WHERE lower(this_.Source) like 'a' and this_.SendDateTime in (SELECT max(this_0_.SendDateTime) as y0_, this_0_.Target as y1_ 
	FROM [DataItem] this_0_ WHERE this_.Source = this_0_.Source GROUP BY this_0_.Target)

The query should be like this (withouth this_0_.Target in subquery)
SELECT this_.ID as ID0_0_, this_.Source as Source0_0_, this_.Target as Target0_0_, this_.SendDateTime as SendDate4_0_0_, this_.Version as Version0_0_ FROM [DataItem] this_ 
WHERE lower(this_.Source) like 'a' and this_.SendDateTime in (SELECT max(this_0_.SendDateTime) as y0_
	FROM [DataItem] this_0_ WHERE this_.Source = this_0_.Source GROUP BY this_0_.Target)
