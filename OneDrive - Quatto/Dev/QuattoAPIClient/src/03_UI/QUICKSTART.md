# ?? Início Rápido - QuattoAPIClient.UI

Comece aqui! Guia rápido para começar em 5 minutos.

---

## ? 5 Passos para Começar

### 1?? **Verificar Compilação** (1 minuto)
```bash
cd src/03_UI
dotnet build
```

**Resultado esperado**: ? Build successful

---

### 2?? **Localizar Documentação** (1 minuto)

Você tem 6 documentos disponíveis:

| Documento | Para | Tempo |
|-----------|------|-------|
| **README.md** | Entender o projeto | 5 min |
| **IMPLEMENTATION_GUIDE.md** | Implementar com SSIS | 30 min |
| **TROUBLESHOOTING.md** | Resolver problemas | 5-15 min |
| **RELEASE_NOTES.md** | Ver roadmap | 10 min |
| **INDEX.md** | Navegar documentação | 5 min |
| **DOCUMENTATION_SUMMARY.md** | Ver sumário | 3 min |

---

### 3?? **Leitura Rápida** (2 minutos)

Escolha seu perfil:

#### ?? Novo Usuário
```
README.md
  ? "Como Usar" section
  ? "Registrar no SSIS Toolbox"
  ? "Configurar o Componente"
```

#### ????? Desenvolvedor
```
IMPLEMENTATION_GUIDE.md
  ? Escolha o Passo que precisa
  ? Copie o código exemplo
  ? Implemente em seu projeto
```

#### ?? Com Problema
```
TROUBLESHOOTING.md
  ? Procure o erro
  ? Siga a solução
  ? Consulte FAQ se necessário
```

---

### 4?? **Próximas Ações** (1 minuto)

**Se compilou com sucesso:**
- ? Projeto está pronto
- ? DLL foi gerada
- ? Pode usar tipos stub ou esperar SSIS

**Se tem SSIS instalado:**
- ?? Siga [IMPLEMENTATION_GUIDE.md](IMPLEMENTATION_GUIDE.md)
- ?? Substitua tipos `object` por tipos reais
- ?? Implemente `LoadCurrentValues()` e `SaveValues()`

**Se encontrou erro:**
- ? Consulte [TROUBLESHOOTING.md](TROUBLESHOOTING.md)
- ? Procure seu erro específico
- ? Siga solução passo-a-passo

---

## ?? Documentação por Tarefa

### "Quero compilar"
? [README.md - Como Usar](README.md#-como-usar)

### "Quero registrar no SSIS"
? [README.md - Registrar no SSIS](README.md#3-registrar-no-ssis-toolbox)

### "Quero implementar com tipos SSIS"
? [IMPLEMENTATION_GUIDE.md](IMPLEMENTATION_GUIDE.md)

### "Tenho erro"
? [TROUBLESHOOTING.md](TROUBLESHOOTING.md)

### "Quero entender o roadmap"
? [RELEASE_NOTES.md](RELEASE_NOTES.md)

### "Quero navegar documentação"
? [INDEX.md](INDEX.md)

---

## ?? Comandos Rápidos

### Compilar
```bash
cd src/03_UI
dotnet build
```

### Compilar Release
```bash
dotnet build -c Release
```

### Limpar
```bash
dotnet clean
```

### Ver referências
```bash
dotnet list package
```

---

## ?? Pré-requisitos

- ? .NET Framework 4.7.2+
- ? Visual Studio 2019+
- ? (Opcional) SQL Server 2019+ com SSIS

Para SSIS reais:
```powershell
# Verificar instalação
Test-Path "C:\Program Files\Microsoft SQL Server\150\DTS\"
```

---

## ?? Suporte Rápido

| Problema | Solução |
|----------|---------|
| "Build falhou" | Veja [TROUBLESHOOTING.md](TROUBLESHOOTING.md#-erros-de-compilação) |
| "Tipo não encontrado" | Veja [TROUBLESHOOTING.md - CS0246](TROUBLESHOOTING.md#cs0246-o-nome-do-tipo-ou-do-namespace--não-pode-ser-encontrado) |
| "Componente não aparece" | Veja [TROUBLESHOOTING.md - Toolbox](TROUBLESHOOTING.md#componente-não-aparece-na-ssis-toolbox) |
| "Wizard não abre" | Veja [TROUBLESHOOTING.md - Wizard](TROUBLESHOOTING.md#-problemas-com-wizard) |
| "Outra dúvida?" | Veja [TROUBLESHOOTING.md - FAQ](TROUBLESHOOTING.md#-faq) |

---

## ?? Roadmap de Ações

```
Agora (v1.0.0 ?)
  ?? Compilar ?
  ?? Registrar no SSIS ??
  ?? Usar como é ??

Quando SSIS Instalado (v1.1.0 ??)
  ?? Tipos reais SSIS
  ?? UI completa
  ?? Persistência de dados

Futuro (v1.2.0 ??)
  ?? Preview de dados
  ?? Schema mapping visual
  ?? Cloud support

Além (v2.0.0 ??)
  ?? .NET 6+ support
  ?? Web config
  ?? Full cloud-native
```

---

## ?? Documentação Completa

Você tem acesso a:

| Documento | Descrição | Tamanho |
|-----------|-----------|---------|
| README.md | Visão geral | 8 KB |
| IMPLEMENTATION_GUIDE.md | Como implementar | 14 KB |
| TROUBLESHOOTING.md | Resolver problemas | 11 KB |
| RELEASE_NOTES.md | Roadmap & versões | 7 KB |
| INDEX.md | Navegação | 9 KB |
| DOCUMENTATION_SUMMARY.md | Sumário | 6 KB |
| **QUICKSTART.md** | Este arquivo | 3 KB |

**Total**: 60+ KB de documentação profissional

---

## ? Checklist Rápido

- [ ] Compilei o projeto com `dotnet build`
- [ ] Vi mensagem "Build successful"
- [ ] Verifiquei que DLL foi gerada em `bin/Debug/net472/`
- [ ] Li a documentação relevante
- [ ] Entendo a estrutura do projeto
- [ ] Sei onde ir se tiver problema

---

## ?? Próximo Passo

Escolha uma opção abaixo:

### ?? "Quero entender tudo"
Leia [README.md](README.md) (10 minutos)

### ?? "Quero implementar agora"
Vá para [IMPLEMENTATION_GUIDE.md](IMPLEMENTATION_GUIDE.md) (30 minutos)

### ?? "Tenho um problema"
Procure em [TROUBLESHOOTING.md](TROUBLESHOOTING.md) (5 minutos)

### ?? "Quero navegar"
Use [INDEX.md](INDEX.md) para encontrar tópicos

### ?? "Preciso de ajuda"
Email: erton.miranda@quatto.com.br

---

## ?? Você Está Aqui

```
?? QUICKSTART.md (Você está aqui)
   ?
?? README.md (Leia depois)
   ?
?? IMPLEMENTATION_GUIDE.md (Se vai implementar)
   ?
?? TROUBLESHOOTING.md (Se tiver problema)
   ?
?? INDEX.md (Para navegar)
```

---

**Status**: ? Pronto para começar  
**Tempo**: ~5 minutos para leitura + setup  
**Próximo**: Escolha uma ação acima

