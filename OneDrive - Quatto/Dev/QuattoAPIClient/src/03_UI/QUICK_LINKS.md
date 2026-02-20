# 🔗 Quick Links & Navigation

> Mapa de navegação rápida para todos os documentos do projeto

---

## 🎯 Comece Aqui

### 👤 Se você é **NOVO**
1. Leia: **[MAIN_README.md](MAIN_README.md)** (5 min)
2. Siga: **[Quick Start](MAIN_README.md#-quick-start-5-minutos)** (5 min)
3. Explore: **[Estrutura do Projeto](MAIN_README.md#-estrutura-do-projeto)** (5 min)

### 🏗️ Se você é **ARQUITETO**
1. Leia: **[ARCHITECTURE.md](ARCHITECTURE.md)** (15 min)
2. Estude: **[Componentes Principais](ARCHITECTURE.md#-componentes-principais)** (10 min)
3. Revise: **[Decisões de Design](ARCHITECTURE.md#-decisões-de-design)** (10 min)

### 🔧 Se você é **DEVOPS/DBA**
1. Leia: **[INSTALLATION.md](INSTALLATION.md)** (10 min)
2. Siga: **[Instalação em Produção](INSTALLATION.md#-instalação-em-produção)** (15 min)
3. Consulte: **[Troubleshooting](INSTALLATION.md#-troubleshooting)** (as needed)

### 🧪 Se você é **QA/TESTER**
1. Leia: **[README_TESTS.md](README_TESTS.md)** (10 min)
2. Aprenda: **[Como Executar Testes](README_TESTS.md#-como-executar-testes)** (5 min)
3. Execute: **[dotnet test](README_TESTS.md#-via-command-line)** (2 min)

### 👨‍💻 Se você é **DESENVOLVEDOR**
1. Leia: **[MAIN_README.md](MAIN_README.md)** (5 min)
2. Setup: **[INSTALLATION.md Dev Setup](INSTALLATION.md#-instalação-para-desenvolvimento)** (15 min)
3. Aprenda Logging: **[LOGGING_GUIDE.md](LOGGING_GUIDE.md)** (10 min)
4. Debug: **[TEST_IN_VISUAL_STUDIO.md](TEST_IN_VISUAL_STUDIO.md)** (5 min)

---

## 📚 Documentação Completa

### 🎯 Visão Geral & Quick Start
- **[MAIN_README.md](MAIN_README.md)** - Entrypoint principal
  - Visão geral do projeto
  - Quick start em 5 minutos
  - Stack tecnológico
  - Links para documentação

### 🏗️ Arquitetura & Design
- **[ARCHITECTURE.md](ARCHITECTURE.md)** - Documentação técnica
  - 4 camadas de arquitetura
  - Componentes principais
  - Fluxos de dados
  - Padrões de design
  - Decisões arquiteturais
  - Considerações de segurança

### 🔧 Setup & Instalação
- **[INSTALLATION.md](INSTALLATION.md)** - Guia passo-a-passo
  - Pré-requisitos
  - Setup para desenvolvimento
  - Setup para produção
  - Verificação
  - 15+ cenários de troubleshooting
  - Checklist de instalação

### 🧪 Testes & QA
- **[README_TESTS.md](README_TESTS.md)** - Documentação de testes
  - 47 testes unitários
  - Como executar testes
  - Padrão AAA
  - Convenções de naming
  - Exemplos de testes
  - Como adicionar novos testes

- **[TEST_IN_VISUAL_STUDIO.md](TEST_IN_VISUAL_STUDIO.md)** - Guia VS
  - Abrir Visual Studio
  - Clean Solution
  - Rebuild All
  - Verificação final
  - Troubleshooting

### 📊 Logging Estruturado
- **[LOGGING_GUIDE.md](LOGGING_GUIDE.md)** - Sistema de logging
  - Como usar LoggerFactory
  - Extensões disponíveis
  - Níveis de log
  - Boas práticas
  - 5+ exemplos práticos
  - Configuração por ambiente

### 📊 Resumo & Dashboard
- **[PROJECT_SUMMARY.md](PROJECT_SUMMARY.md)** - Resumo executivo
  - Overview completo
  - Fases do projeto
  - Métricas finais
  - Checklist
  - Status de produção

- **[DASHBOARD.md](DASHBOARD.md)** - Dashboard visual
  - Progress bars
  - Estatísticas por fase
  - Deliverables
  - Quality metrics
  - Timeline
  - Key achievements

---

## 🎯 Por Tópico

### Começar

```
MAIN_README.md
└─ Quick Start
   └─ 1. Clonar
   └─ 2. Build
   └─ 3. Testes
   └─ 4. Usar em SSIS
```

### Entender Arquitetura

```
ARCHITECTURE.md
├─ 4 Camadas
│  ├─ UI Layer (CorporateApiSourceUI)
│  ├─ Component Layer (CorporateApiSource)
│  ├─ Connection Layer (ApiConnectionManager)
│  └─ Transport Layer (HTTP/HTTPS)
├─ Fluxos de Dados
├─ Padrões de Design
└─ Decisões Arquiteturais
```

### Instalar

```
INSTALLATION.md
├─ Pré-requisitos
├─ Dev Setup (6 passos)
├─ Prod Setup (6 passos)
├─ Verificação (3 testes)
└─ Troubleshooting (15+ problemas)
```

### Testar

```
TEST_IN_VISUAL_STUDIO.md + README_TESTS.md
├─ Abrir VS
├─ Clean Solution
├─ Rebuild
├─ Run Tests (47)
└─ Verificar Resultados
```

### Debugar

```
LOGGING_GUIDE.md
├─ LoggerFactory singleton
├─ LoggerExtensions
├─ LogScope para contexto
├─ 5 exemplos práticos
└─ Boas práticas
```

---

## 🔍 Buscar por Tópico

### Logging
- 📖 [LOGGING_GUIDE.md](LOGGING_GUIDE.md) - Guia completo
- 📖 [ARCHITECTURE.md - Logging Pattern](ARCHITECTURE.md#-logging-pattern-observer)
- 📖 [MAIN_README.md - Logging](MAIN_README.md#-logging-estruturado-integrado)

### Testes
- 📖 [README_TESTS.md](README_TESTS.md) - Documentação principal
- 📖 [TEST_IN_VISUAL_STUDIO.md](TEST_IN_VISUAL_STUDIO.md) - Setup VS
- 📖 [ARCHITECTURE.md - Testes por Camada](ARCHITECTURE.md#-testes-por-camada)

### SSIS
- 📖 [ARCHITECTURE.md - Componentes](ARCHITECTURE.md#-componentes-principais)
- 📖 [INSTALLATION.md - Pré-requisitos](INSTALLATION.md#-pré-requisitos)
- 📖 [MAIN_README.md - Como Usar em SSIS](MAIN_README.md#-4-usar-em-ssis)

### Segurança
- 📖 [ARCHITECTURE.md - Segurança](ARCHITECTURE.md#-segurança)
- 📖 [INSTALLATION.md - Troubleshooting](INSTALLATION.md#-troubleshooting)

### Performance
- 📖 [ARCHITECTURE.md - Performance](ARCHITECTURE.md#-performance)
- 📖 [LOGGING_GUIDE.md - Configuração Avançada](LOGGING_GUIDE.md#-configuração-avançada)

### Troubleshooting
- 📖 [INSTALLATION.md - Troubleshooting](INSTALLATION.md#-troubleshooting)
- 📖 [TEST_IN_VISUAL_STUDIO.md - Erros Comuns](TEST_IN_VISUAL_STUDIO.md#-se-tiver-erros)

---

## 📋 Checklists Rápidos

### ✅ Setup Checklist

```
Dev Environment:
□ .NET Framework 4.7.2
□ SQL Server 2022
□ SSIS v17.100
□ Visual Studio 2022 18.3.1+
□ Projeto clonado
□ NuGet restored
□ Solução compila (Ctrl+Shift+B)
□ 47 testes passam

Prod Environment:
□ DLLs compiladas (Release)
□ Copiadas para SSIS Binn
□ Toolbox atualizado
□ Wizard funciona
□ Config salva corretamente
□ Package executa OK
□ Logging funciona
```

### ✅ Test Checklist

```
□ Abrir Test Explorer (Ctrl+E, T)
□ Run All Tests
□ Esperado: 47 passed
□ Zero failed
□ Cobertura > 70%
□ Logging aparece no output
```

### ✅ Deploy Checklist

```
□ Build Release criado
□ DLLs compiladas
□ Dependências listadas
□ Teste em dev OK
□ Teste em staging OK
□ Documentação revisada
□ Rollback plan pronto
□ Monitoring ativado
```

---

## 🚀 Comandos Frequentes

### Visual Studio
```powershell
Ctrl+Shift+B          # Build Solution
Ctrl+E, T             # Open Test Explorer
Ctrl+R, A             # Run All Tests
F5                    # Start Debugging
Ctrl+K, C             # Comment Code
Ctrl+K, U             # Uncomment Code
```

### Command Line
```powershell
# Build
dotnet build

# Test
dotnet test 04_Tests/QuattoAPIClient.Tests.csproj

# Test with verbosity
dotnet test 04_Tests/ -v d

# Test specific class
dotnet test 04_Tests/ -k "LoggerFactory"

# Coverage
dotnet test /p:CollectCoverage=true
```

### Git
```powershell
git clone <repo>
git status
git add .
git commit -m "message"
git push origin main
```

---

## 📞 Contato & Suporte

### Documentação
- 📖 [Todos os Guias](.)
- 🔍 [Search Docs](.)
- 📚 [Índice Completo](.)

### Comunicação
- 📧 **Email:** support@quatto.com.br
- 🐛 **Issues:** [GitHub Issues](https://github.com/quatto)
- 💬 **Discussions:** [GitHub Discussions](https://github.com/quatto)

### Desenvolvedor
- 👤 **Erton Miranda** / Quatto Consultoria
- 🌐 https://www.quatto.com.br

---

## 📊 Índice Rápido

### Arquivos do Projeto
```
src/
├── 01_Source/           (Componente SSIS)
├── 02_ConnectionManager (Connection Manager)
├── 03_UI/              (Interface Designer)
└── 04_Tests/           (Testes - 47)
```

### Documentação
```
Raiz/
├── MAIN_README.md       (👈 Start here!)
├── ARCHITECTURE.md
├── INSTALLATION.md
├── LOGGING_GUIDE.md
├── README_TESTS.md
├── TEST_IN_VISUAL_STUDIO.md
├── PROJECT_SUMMARY.md
├── DASHBOARD.md
└── QUICK_LINKS.md       (Este arquivo)
```

---

## 🎓 Learning Path

### Para Iniciantes
1. **MAIN_README.md** (5 min) - Visão geral
2. **INSTALLATION.md Dev** (15 min) - Setup
3. **LOGGING_GUIDE.md** (10 min) - Exemplos
4. **README_TESTS.md** (10 min) - Como testar

### Para Intermediários
1. **ARCHITECTURE.md** (20 min) - Padrões e design
2. **CODE** - Estudar source
3. **README_TESTS.md** (20 min) - Escrever testes
4. **LOGGING_GUIDE.md** (20 min) - Implementar logging

### Para Avançados
1. **ARCHITECTURE.md completo** (30 min)
2. **Code Review** - Revisar implementações
3. **Criar extensões** - Custom handlers
4. **Otimizações** - Performance tuning

---

## ⭐ Destaques

### 🏆 Must-Read Docs
- [MAIN_README.md](MAIN_README.md) - Visão geral
- [ARCHITECTURE.md](ARCHITECTURE.md) - Técnico
- [INSTALLATION.md](INSTALLATION.md) - Setup

### 🎓 Must-Know Concepts
- Logging estruturado (LOGGING_GUIDE.md)
- 47 testes (README_TESTS.md)
- 4 camadas (ARCHITECTURE.md)
- OAuth2 refresh (ARCHITECTURE.md)

### ⚠️ Must-Avoid Mistakes
- Usar dotnet CLI para build (use VS)
- Ignorar logging (crucial para debug)
- Não rodar testes antes de deploy
- Esquecer de atualizar documentação

---

**Última Atualização:** 2026-02-20  
**Status:** ✅ Completo  
**Version:** 1.0.0

🎉 **Obrigado por usar Quatto API Client for SSIS!**

