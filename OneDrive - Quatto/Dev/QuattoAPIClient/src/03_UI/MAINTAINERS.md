# 👨‍💼 Maintainers & Leadership

> Informações sobre mantedores, processo de contribuição e como se tornar maintainer

**Versão:** 1.0.0  
**Data:** 2025-02-20  
**Status:** Ativo

---

## 👥 Maintainers Atuais

### Lead Maintainer

**Erton Miranda**
- **GitHub:** [@ertonjm](https://github.com/ertonjm)
- **Email:** erton.miranda@quatto.com.br
- **Responsabilidades:**
  - Decisões arquiteturais
  - Aprovação de PRs maiores
  - Planejamento de roadmap
  - Releases e versioning
  - Segurança e compliance
  - Comunicação com comunidade

**Disponibilidade:**
- 📧 Email: Resposta em 24-48h
- 💬 GitHub Discussions: Resposta em 24-48h
- 🔔 Security Issues: Resposta em 24h (máximo)

---

## 🏢 Organização & Empresa

**Quatto Consultoria**
- **Website:** https://www.quatto.com.br
- **Email:** support@quatto.com.br
- **Especialização:** SSIS, SQL Server, ETL, Data Integration

**Responsabilidades da Empresa:**
- ✅ Manutenção principal do projeto
- ✅ Desenvolvimento de features principais
- ✅ Suporte prioritário a clientes
- ✅ Compliance e segurança
- ✅ Infraestrutura (hosting, CI/CD)

---

## 🎖️ Estrutura de Contribuidores

### 1. **Committer** (Acesso de escrita)

**Requisitos:**
- ✅ 5+ PRs aceitas
- ✅ 3+ meses de contribuição consistente
- ✅ Profundo conhecimento da codebase
- ✅ Adesão a code of conduct
- ✅ Passar em technical interview

**Responsabilidades:**
- ✅ Revisar PRs de outros
- ✅ Mergear PRs aprovadas
- ✅ Ajudar na triage de issues
- ✅ Manutenção de code quality
- ✅ Mentorship de novos contributors

**Privilégios:**
- ✅ Acesso de escrita ao repositório
- ✅ Poder revisar e mergear PRs
- ✅ Acesso ao Slack/Discord privado
- ✅ Menção em AUTHORS.md

### 2. **Reviewer** (Sem acesso de escrita)

**Requisitos:**
- ✅ 3+ PRs aceitas
- ✅ 1+ mês de contribuição
- ✅ Expertise em área específica
- ✅ Vontade de revisar código

**Responsabilidades:**
- ✅ Revisar PRs na sua área
- ✅ Dar feedback construtivo
- ✅ Sugerir melhorias

**Privilégios:**
- ✅ Requests de review automáticos
- ✅ Menção em CONTRIBUTORS.md

### 3. **Contributor** (Comunidade)

**Como começar:**
- ✅ Fork repositório
- ✅ Faça uma mudança
- ✅ Abra Pull Request
- ✅ Responda feedback

**Recursos:**
- ✅ [GETTING_STARTED.md](GETTING_STARTED.md)
- ✅ [CONTRIBUTING.md](CONTRIBUTING.md)
- ✅ [GIT_WORKFLOW.md](GIT_WORKFLOW.md)

---

## 📋 Processo de Aprovação

### Para PRs Normais (< 100 linhas)
```
PR aberto
  ↓
Assignee automático: @ertonjm
  ↓
CI/CD checks (automático)
  ↓
Code review (48h)
  ↓
Feedback ou aprovação
  ↓
Merge (squash)
```

### Para PRs Grandes (> 100 linhas)
```
PR aberto
  ↓
Assignee automático: @ertonjm + 1 reviewer
  ↓
CI/CD checks (automático)
  ↓
Code review (72h)
  ↓
Technical discussion
  ↓
Feedback e iteração
  ↓
Aprovação de 2 pessoas
  ↓
Merge (squash)
```

### Para Breaking Changes
```
Discussion em GitHub Discussions (1+ semana)
  ↓
RFC (Request for Comments) criado
  ↓
Community feedback (2+ semanas)
  ↓
Decisão tomada
  ↓
PR com RFC reference
  ↓
Code review (1+ semana)
  ↓
Aprovação de lead maintainer
  ↓
Merge (com deprecation warning em v anterior)
```

---

## 🔄 Processo de Release

### Pre-Release (RC)
```
1. Create release/v1.1.0 branch
2. Update CHANGELOG.md
3. Update version in code
4. Create pre-release tag: v1.1.0-rc.1
5. Run full test suite
6. Community testing (1+ semana)
7. Feedback incorporated
```

### Release Final
```
1. Create PR: release/v1.1.0 → main
2. Code review
3. Final testing
4. Merge to main
5. Tag: v1.1.0
6. Create GitHub Release
7. Deploy artifacts
8. Announce release
9. Merge back to develop
```

### Post-Release
```
1. Document any issues
2. Plan patch if needed
3. Close related issues
4. Thank contributors
5. Update roadmap if needed
```

---

## 📊 Decision Making

### Trivial Decisions (< 2h trabalho)
- Lead maintainer decide sozinho
- Notifica comunidade

### Normal Decisions (2-16h trabalho)
- Lead maintainer + 1 reviewer
- Discussão em PR
- Vote if disagreement

### Major Decisions (> 16h trabalho ou breaking)
- GitHub Discussion aberto
- RFC (Request for Comments)
- Comunidade vote
- Lead maintainer final call

### Emergência (Security, Critical Bug)
- Lead maintainer decide imediatamente
- Patch lançado ASAP
- Post-mortem depois

---

## 🚀 Como se Tornar Committer

### Passo 1: Contribuir Consistentemente
```
Objetivo: 5+ PRs aceitas em 3+ meses
Exemplo:
- PR 1: Documentação
- PR 2: Bug fix pequeno
- PR 3: Feature pequena
- PR 4: Tests
- PR 5: Refactor
```

### Passo 2: Ganhar Reputação
```
Objetivo: Ser conhecido positivamente
- Boa qualidade de código
- Receptivo a feedback
- Adesão a code of conduct
- Ajuda outros contributors
```

### Passo 3: Technical Interview
```
Tópicos cobertos:
- Conhecimento da arquitetura
- Padrões de design usados
- Considerações de segurança
- Performance optimization
- Decisions logging
```

### Passo 4: Aprovação
```
1. Propor ao lead maintainer
2. Lead approva proposta
3. Vote da comunidade (opcional)
4. Anúncio público
5. Update README e AUTHORS
```

---

## 💼 Diretrizes de Comportamento

### O Que Esperamos
- ✅ Profissionalismo
- ✅ Respeito mútuo
- ✅ Comunicação clara
- ✅ Honestidade
- ✅ Transparência
- ✅ Receptivo a feedback
- ✅ Leitura do Code of Conduct

### O Que Não Toleramos
- ❌ Assédio ou abuso
- ❌ Discriminação
- ❌ Spam ou self-promotion
- ❌ Violação de privacidade
- ❌ Conflicts of interest não declarados
- ❌ Violação de licença

### Processo Disciplinar
```
1. Warning (privado)
2. Public warning (se continuar)
3. Restrição temporária
4. Ban permanente (último recurso)
```

---

## 📞 Contatos & Escalação

### Questões Gerais
- 📧 Email: support@quatto.com.br
- 💬 GitHub Discussions
- 🐛 GitHub Issues

### Security Issues
- 🔐 GitHub Security Advisory
- 📧 Email: security@quatto.com.br
- ⏰ Resposta: 24h máximo

### Complaints ou Conflicts
- 📧 Email privado: erton.miranda@quatto.com.br
- 🔒 Confidential
- 📋 Investigação imparcial

### Sponsorship ou Partnerships
- 📧 Email: support@quatto.com.br
- 💼 Business development
- 🤝 Discutir oportunidades

---

## 📚 Recursos para Maintainers

### Setup Local
```powershell
# Clone com acesso de escrita
git clone https://github.com/ertonjm/QuattoAPIClient.git --depth 1

# Configure git
git config user.name "Your Name"
git config user.email "your@email.com"

# Add upstream
git remote add upstream https://github.com/ertonjm/QuattoAPIClient.git
```

### Reviewer Checklist
```
Code Quality:
  [ ] Sem warnings
  [ ] Segue style guide
  [ ] Bem estruturado
  [ ] Sem código morto

Functionality:
  [ ] Faz o que promete
  [ ] Handles edge cases
  [ ] Testes adequados
  [ ] Performance ok

Documentation:
  [ ] Comments úteis
  [ ] XML docs
  [ ] README atualizado
  [ ] CHANGELOG atualizado

Security:
  [ ] Sem hardcoded secrets
  [ ] Input validation
  [ ] Dependency check
  [ ] No known vulns
```

### Mergear PR
```powershell
# Checkout branch
git checkout feature-branch

# Merge squash (recomendado para historia limpa)
git merge --squash develop

# Commit com mensagem conventionl
git commit -m "feat(scope): description"

# Ou merge normal
git merge develop
```

---

## 📊 Estadísticas Esperadas

### Resposta Time
```
Normal PR:        < 48h
Urgent PR:        < 24h
Security issue:   < 24h (MUST)
Discussion:       < 48h
```

### Review Quality
```
Feedback útil:    100%
Respeitoso:       100%
Ações on follow:  100%
```

### Community Health
```
Issues closed:    > 90%
PRs merged:       > 90%
Response time:    < 48h avg
Contributor growth: +50% YoY
```

---

## 🔄 Atualização de Maintainers

### Quando Adicionar
- Novo committer pronto (passou por processo)
- Expertise em nova área
- Reduzir carga do lead maintainer

### Quando Remover
- Inatividade > 6 meses (aviso primeiro)
- Violação do Code of Conduct
- Conflito de interesse não resolvido

### Comunicação
- ✅ Anúncio público quando adiciona
- ✅ Aviso privado quando remove
- ✅ Update README e AUTHORS

---

## 📝 Historico de Maintainers

| Nome | Período | Rol | Status |
|------|---------|-----|--------|
| Erton Miranda | 2025-present | Lead Maintainer | ✅ Ativo |

---

## 🌟 Reconhecimento

### No README.md
```markdown
## 👨‍💼 Maintainers

- **Erton Miranda** - Lead maintainer
  - GitHub: [@ertonjm](https://github.com/ertonjm)
  - Email: erton.miranda@quatto.com.br
```

### No AUTHORS.md
```markdown
## Active Maintainers

- Erton Miranda (@ertonjm)

## Contributors

[Lista de todos que contribuíram]
```

### No CHANGELOG
```markdown
## Credits

Obrigado a:
- Nomes de contributors importantes
- Security researchers
- Community members
```

---

## 📚 Referências

- [Opensource Governance](https://opensource.guide/)
- [Maintainer Responsibilities](https://github.com/maintainers)
- [Community Building](https://opensource.guide/building-community/)
- [Leadership in Open Source](https://hacks.mozilla.org/2021/05/celebrating-open-source-leaders/)

---

## 📞 Próximos Passos

### Para Novo Contributors
1. Leia [CONTRIBUTING.md](CONTRIBUTING.md)
2. Faça primeira PR
3. Ganhe experiência
4. Considere se tornar committer

### Para Potential Maintainers
1. Contribua regularmente
2. Ganhe reputação
3. Contate lead maintainer
4. Prepare para interview

### Para Empresas
1. Email: support@quatto.com.br
2. Discuta sponsorship
3. Explore partnerships
4. Considere contribution

---

**Versão:** 1.0.0  
**Data:** 2025-02-20  
**Mantido por:** @ertonjm  
**Status:** Ativo e aberto a crescimento! 🚀

