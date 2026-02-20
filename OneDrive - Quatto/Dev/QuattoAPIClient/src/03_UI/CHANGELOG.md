# Changelog

All notable changes to Quatto API Client for SSIS will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2026-02-20

### Added
- Initial release of Quatto API Client for SSIS
- SSIS v17.100 (SQL Server 2022) support
- Comprehensive logging via Microsoft.Extensions.Logging
- OAuth2 authentication with automatic token refresh
- Bearer Token and API Key authentication support
- Incremental data loading with watermark support
- Paging support for large datasets
- Rate limiting and timeout configuration
- Configurable retry with exponential backoff
- Complete UI wizard in SSIS Designer
- 47 unit tests with xUnit framework
- GitHub Actions CI/CD pipeline
- Azure DevOps pipeline (alternative)
- Comprehensive technical documentation (11 guides)
- Code quality scanning with SonarCloud
- Security scanning with Trivy

### Technical Details
- **Platform:** .NET Framework 4.7.2
- **Architecture:** 4-layer SSIS component
- **Logging:** Microsoft.Extensions.Logging 8.0.0
- **Testing:** xUnit 2.6.6 + Moq 4.20.70
- **Code Coverage:** 70%+
- **Quality Grade:** A+ (90%+)

### Documentation
- Quick start guide
- Architecture documentation
- Installation & troubleshooting guide
- Logging implementation guide
- 47 unit tests documentation
- CI/CD pipeline setup guide
- Release notes template

### Known Limitations
- Requires SQL Server 2022 with SSIS v17.100
- .NET Framework 4.7.2+ required
- UI only available in SSIS Designer (not SSDT Business Intelligence)

### Contributors
- Erton Miranda / Quatto Consultoria

---

## Planned for Future Releases

### v1.1.0 (Planned)
- [ ] GraphQL query support
- [ ] Advanced caching mechanisms
- [ ] Circuit breaker pattern implementation
- [ ] Performance metrics collection
- [ ] Additional authentication providers
- [ ] Batch operation support

### v1.2.0 (Planned)
- [ ] Integration tests
- [ ] Performance benchmarks
- [ ] Sample projects with examples
- [ ] Video tutorials
- [ ] Community contribution guidelines
- [ ] Advanced logging scenarios

### v2.0.0 (Future)
- [ ] .NET 6+ migration
- [ ] SSIS 2024+ support
- [ ] GraphQL full support
- [ ] gRPC support
- [ ] Plugin architecture
- [ ] Web dashboard for monitoring

---

## How to Report Issues

If you encounter any issues, please:

1. Check the [TROUBLESHOOTING.md](TROUBLESHOOTING.md) guide
2. Review existing GitHub issues
3. Create a new issue with:
   - Clear description of the problem
   - Steps to reproduce
   - Expected vs actual behavior
   - Your environment (OS, SQL Server version, etc.)
   - Relevant logs

---

## How to Contribute

See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

---

## Version History

| Version | Date | Status |
|---------|------|--------|
| 1.0.0 | 2026-02-20 | ✅ Released |
| 1.1.0 | TBD | ⏳ Planned |
| 1.2.0 | TBD | ⏳ Planned |

---

**Last Updated:** 2026-02-20  
**Maintained by:** Quatto Consultoria

