# Split Csv file by OrderId

### Ops Request
	-	Can you look at a way of segregating the EDI orders for frozen and reach out to Azyra for a route 
	we can use to have frozen stock on our system and we can have 2 accounts pulling from this stock which 
	are listed below so we can full automate the charges for these 2 customers as the time spent manually 
	separating these orders at present is very Time Consuming in relation to separating the 2 charge sheets 
	for each account and manually updating the freight jobs to show the correct account .  

### IT Requirements
	1.	Write a code to split main EDI file (.csv) into two (one for each customer) by OrderId column.
	2.	Alocate files into relevat receive EDI folders.
		a. SORIE to Acc 1 EDI folder.
		b. SORGB to Acc 2 EDI folder.
	3.	Move original EDI file into Archive folder.
		a. Acc 1 subfolder "Archive".
	4.	Upload existing stockodes from customer Acc 1 to customer Acc 2.
	5.	Upload address data translation to customer Acc 2.

### Code requirements
	1. Check if provided file paths exists, except archive folder. [x]
		a. If one of paths null - terminate: send email notification, save error to the log file.
	2. Get all .csv files from the input folder into array. [x]
		a. If array lengt equals 0 - terminate.
	3. Slit main EDI file in to two EDI files. [x]
		a. Split data by csv column 4 (OrderId), sorting option: each row column OrderId contains SORIE or SORGB.
		b. Save each file with a relevant SORIE/SORGB add on to the original file name. 
	4. Move original EDI to the archive folder. [x]
	5. Implement error log function. [x]
	6. Implement email error notification. [x]
