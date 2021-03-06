SELECT A.firstName AS [FirstName], 
		A.lastName AS [LastName],
		'CTec' + CAST(C.id AS varchar(10)) AS [CourseID],
		LEFT(C.title, 252) AS [CourseTitle],
		MAX(SCA.timestamp) AS [CompletedDate],
		A.email as Email,
		CAST(SE.score AS decimal(10,2)) as Score
FROM CTEC_StudentCourseAction SCA
JOIN CTEC_Course C
	ON SCA.courseID = C.id
JOIN CTEC_Student S
	ON SCA.studentID = S.id
JOIN CTEC_Account A
	ON S.onTapAccountID = A.onTapID
JOIN (SELECT AVG(CAST(sc.score AS float)) score, sc.studentID, sc.courseID from
		(select max(score) score, studentID, courseID, moduleID from CTEC_StudentEvaluation 
		where isMasteryAchieved = 1
		group by studentID, courseID, moduleID) sc
		group by sc.studentID, sc.courseID) SE
	ON SE.studentID = SCA.studentID
		AND SE.courseID = SCA.courseID
WHERE actionTypeID = 11
	AND SCA.timestamp >= '06/01/2019'
GROUP BY A.firstName, A.lastName, 'CTec' + CAST(C.id AS varchar(10)), LEFT(C.title, 252), A.email, CAST(SE.score AS decimal(10,2)) 

