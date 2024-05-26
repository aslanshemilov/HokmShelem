﻿global using Api.Extensions;
global using Api.Middleware;
global using AutoMapper;
global using Api.Data;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using System;
global using System.Net;
global using System.Text.Json;
global using System.Threading.Tasks;
global using Api.Dtos.Account;
global using Api.Dtos.Profile;
global using StaticDetails;
global using System.Linq;
global using Api.Repository;
global using Api.Services;
global using Microsoft.Extensions.Configuration;
global using System.IdentityModel.Tokens.Jwt;
global using System.Security.Claims;
global using Microsoft.IdentityModel.Tokens;
global using System.Text;
global using System.Collections.Generic;
global using AutoMapper.QueryableExtensions;
global using Google.Apis.Auth;
global using StaticDetail;
global using System.Net.Http;
global using System.Net.Http.Json;
global using Api.Dtos.Admin;
global using System.ComponentModel.DataAnnotations;
global using Api.IRepository;
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
global using Microsoft.AspNetCore.Identity;
global using System.ComponentModel.DataAnnotations.Schema;
global using Api.Models;
global using System.IO;
global using Api.IServices;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.WebUtilities;
global using Microsoft.AspNetCore.Http;
global using Api.Utility.Options;
global using Mailjet.Client;
global using Mailjet.Client.TransactionalEmails;
global using Microsoft.Extensions.Options;
global using SendGrid;
global using SendGrid.Helpers.Mail;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.Extensions.Hosting;
global using Api.Models.HokmShelem;
global using System.Net.Mail;
global using Api.Dtos.HokmShelem;
global using Api.Dtos;
global using Microsoft.OpenApi.Models;
global using Api.Repository.IRepository;
global using Api.Dtos.Pagination;
global using Api.Services.IServices;
global using Microsoft.AspNetCore.Authentication;
global using System.Net.Http.Headers;
global using System.Threading;
global using Api.Dtos.GameHistory;
