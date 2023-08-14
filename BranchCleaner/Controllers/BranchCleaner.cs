﻿using AutoMapper;
using BranchCleaner.Models;
using BranchCleanerDll.Interfaces;
using BranchCleanerDll.Model;
using BranchCleanerDll.Repository;
using Microsoft.AspNetCore.Mvc;

namespace BranchCleaner.Controllers
{

    [ApiController]
    public class BranchCleaner : Controller
    {

        private readonly IBranchCleaner _branchCleaner;
        private readonly IMapper _mapper; 
        private readonly ILogger<BranchCleaner> _logger;    

        public BranchCleaner(IBranchCleaner branchCleaner, IMapper mapper, ILogger<BranchCleaner> logger)
        {
            _branchCleaner = branchCleaner;
            _mapper = mapper;
            _logger = logger;   
        }


        [Route("GetBranches")]
        [HttpGet]
       public async Task<IActionResult> GetBranches()
       {
            List<RepoBranchDays> finallList = new List<RepoBranchDays>();
            _logger.LogInformation("Going to fetch the branches from repositories");
            var result = await _branchCleaner.GetBranches("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");
            if(result.Item2 == "")
            {
                _logger.LogInformation("Fetched the branches successfully and now goiong to fetch commit of paticular branch");
                Dictionary<string, List<string>> reposwithbranches = result.Item1 as Dictionary<string, List<string>>;
                foreach (var repowithbranches in reposwithbranches)
                {
                    RepoBranchDays repobranchdays = new RepoBranchDays();
                    string reponame = repowithbranches.Key;
                    _logger.LogInformation("going to fetch from repo: " + reponame +" for this branch commit: "+ reposwithbranches.Values);
                    List<BranchDate> branchdates = await _branchCleaner.GetCommit("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", repowithbranches.Value, "MD TANWEER ALAM", repowithbranches.Key);
                    repobranchdays.repoName = reponame;
                    repobranchdays.branchDays = _mapper.Map<List<BranchDate>, List<BranchDays>>(branchdates);
                    finallList.Add(repobranchdays);
                }
                _logger.LogInformation("returing the final list of user");
                return Ok(new { list = finallList });
            }
            _logger.LogError("Something happen bad with message: " + result.Item2);
           return BadRequest(new { error = result.Item2 });          
        }
    }
}
