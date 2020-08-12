SELECT [recnum] as Record
      ,[invnum] as Invoice
      ,[ctcnum] as SubcontractNum
      ,[vndnum] as Vendor
      ,[jobnum] as Job
      ,[phsnum] as Phase
      ,[dscrpt] as Description
      ,[invdte] as InvoiceDate
      ,[duedte] as DueDate
      ,[dscdte] as Dicovered
      ,[invtyp] as Type
      ,[status] as Status
      ,[retain] as Retention
      ,[amtpad] as Paid
      ,[invttl] as InvoiceTotal
      ,[ttlpad] as TotalPaid
      ,[invbal] as Balance
      ,[invnet] as NetDue
      ,[actper] as Period
      ,[entdte] as Entered
      ,[lgrrec] as LinkNum
      ,[usrnme] as UserName
      ,[btcnum] as BatchNum
      ,[postyr] as PostingYear
  FROM [FirstSampleCompany].[Reports_v1].[acpinv]
