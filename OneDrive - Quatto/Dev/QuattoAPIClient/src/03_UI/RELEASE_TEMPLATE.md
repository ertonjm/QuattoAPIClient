# Release Notes Template

Use este template para criar notas de release quando fazer um novo release.

---

## Release v1.X.X - [DATA]

**Release Date:** [DATE]  
**Version:** 1.X.X  
**Status:** ✅ Available  

---

## 📋 Overview

Uma ou duas linhas descrevendo os principais destaques desta release.

---

## ✨ Features

### Feature 1
- Descrição detalhada
- Benefícios
- Exemplo de uso

### Feature 2
- Descrição detalhada
- Benefícios
- Exemplo de uso

---

## 🐛 Bug Fixes

### Bug 1
- Descrição do problema
- Solução implementada
- Impacto

### Bug 2
- Descrição do problema
- Solução implementada
- Impacto

---

## ⚡ Performance Improvements

- Melhoria 1: ~X% mais rápido
- Melhoria 2: ~Y% menos memória
- Melhoria 3: Otimização Z

---

## 🔒 Security Updates

- Patch para vulnerabilidade X
- Atualização de dependency Y
- Security best practices Z

---

## 📚 Documentation Updates

- Novo guia de instalação
- Exemplos atualizados
- FAQ expandido
- Troubleshooting aprimorado

---

## 📦 Dependencies Updated

| Package | From | To |
|---------|------|-----|
| Microsoft.Extensions.Logging | 8.0.0 | 8.0.1 |
| xUnit | 2.6.6 | 2.7.0 |
| Moq | 4.20.70 | 4.21.0 |

---

## 🔄 Breaking Changes

⚠️ **None** - Fully backward compatible

Or if there are breaking changes:

- **Change 1:** Descrição e migração
- **Change 2:** Descrição e migração

---

## 📊 Metrics

```
Build Status:        ✅ Success
Test Pass Rate:      47/47 (100%)
Code Coverage:       72%
SonarQube Grade:     A
Security Scan:       0 vulnerabilities
Performance:         +5% improvement
```

---

## 📥 Downloads

| Asset | Link |
|-------|------|
| **QuattoAPIClient-1.X.X.zip** | [Download](https://github.com/...) |
| **Documentation** | [PDF](https://github.com/...) |
| **Source Code** | [.zip](https://github.com/...) \| [.tar.gz](https://github.com/...) |

---

## 🚀 Installation & Upgrade

### New Installation

```powershell
# Follow INSTALLATION.md for Dev or Production setup
```

### Upgrade from 1.X-1

```powershell
# 1. Backup current DLLs
Copy-Item "C:\SSIS\Components\QuattoAPIClient.*.dll" -Destination "C:\Backups\SSIS\"

# 2. Extract new DLLs
Expand-Archive "QuattoAPIClient-1.X.X.zip" -DestinationPath "C:\SSIS\Components\"

# 3. Verify
# Open SSDT and check toolbox
```

---

## ✅ Known Issues

- Issue 1: Descrição e workaround
- Issue 2: Descrição e workaround
- Issue 3: Agendado para v1.Y.Z

---

## 🙏 Contributors

- Erton Miranda / Quatto Consultoria
- @user1 (Contribuição X)
- @user2 (Contribuição Y)

---

## 📞 Support

- 📧 **Email:** support@quatto.com.br
- 🐛 **Issues:** [GitHub Issues](https://github.com/...)
- 💬 **Discussions:** [GitHub Discussions](https://github.com/...)
- 📚 **Documentation:** [Wiki](https://github.com/...)

---

## 🔗 Related Resources

- [Installation Guide](INSTALLATION.md)
- [Architecture](ARCHITECTURE.md)
- [Changelog](CHANGELOG.md)
- [Known Issues](https://github.com/.../issues)
- [Upgrade Guide](UPGRADE_GUIDE.md)

---

## 🎯 What's Next?

### v1.X+1 (Next Release)
- [ ] Feature X
- [ ] Performance improvement Y
- [ ] Security update Z

---

## ⚙️ For Developers

### Build from Source

```powershell
git clone <repo>
cd src
dotnet build -c Release
dotnet test 04_Tests/
```

### Contribute

See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

---

**Thank you for using Quatto API Client for SSIS!** 🙏

