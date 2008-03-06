<?xml version="1.0"?>
<xsl:transform xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  <!-- what should happen when we have two different parts of speech, each
  within a single entry? should they have sense numbers in front of the part of speech?
  Should sense numbers be restarted after the part of speech?
  should they be considered homographs?-->

<!-- one way to handle example sentences and their translations is to use the
following format: <a pie: on foot> or more generally, use a colon between forms.
-->


  <!--
  Ideas taken from:

  Bruderlin, Christine. 2004. User-Friendly Dictionary Design.
  http://www.anu.edu.au/linguistics/nash/lexicog/Dict_design_notes.pdf

  Thomson, Mark. 2006. "Microtypography: Designing the new Collins dictionaries." Typo (19). February 2006. pp. 16-19.
  http://www.svettisku.cz/buxus/docs/TYPO_2006_19.pdf?buxus_svettisku=b1cf83
  http://www.typotheque.com/articles/microtypography_designing_the_new_collins_dictionaries/

  Dictionaries on Computer.
  http://www.tu-chemnitz.de/phil/english/chairs/linguist/real/independent/llc/Conference1998/Papers/Nesi.htm
  Wanted:
  Dubois, J. 1981. Models of the Dictionary: Evolution in Dictionary Design
  Applied Linguistics. 1981; II: 236-249

  Text Representation, Dictionary Structure, and Lexical knowledge
BOGURAEV and NEFF Lit Linguist Computing.1992; 7: 110-112
  -->
  <xsl:output method="html" indent="yes" encoding="utf-8"/>

  <xsl:param name="writing-system-info-file" select="'writingSystemPrefs.xml'"/>
  <xsl:param name="grammatical-info-optionslist-file"/>
  <xsl:param name="relations-optionslist-file"/>
  <!-- things like 'see' for cross-references and 'ant' for antonyms, 'syn' for synonyms, etc.-->
  <xsl:param name="optionslist-writing-system" select="'en'"/>

  <xsl:param name="headword-writing-system" select="//entry/field[@tag='headword']/form/@lang"/>

  <xsl:param name="include-notes" select="false()"/>
  <xsl:param name="group-by-grammatical-info" select="true()"/>

  <xsl:variable name="entries-with-BaseForm-relation-rendered-as-subentries-of-base" select="true()"/>
  <xsl:variable name="output-intented-for-winword" select="false()"/>
  <xsl:template match="/">
	<html>
	  <head>
		<xsl:if test="not($output-intented-for-winword)">
		  <link rel="stylesheet" href="user.css">
		  </link>
		</xsl:if>
		<style type="text/css">
		  div.entry { /* this gives us outdented headwords */
		  text-indent: -.5em; /* a slight outdent the width of a lower-case n */
		  margin-left: .5em;
		  line-height: 120%;
		  }

		  /* use sans serif for related words */

		  span.headword {
		  /* Contemporary dictionaries generally tend to use
		  lower- or upper and lower-case forms for headwords,
		  in a bolder and often sans serif type, at a
		  slightly larger size than the entries. */

		  font-family:sans-serif; /*to do: select the sans-serif variety from writing system*/
		  font-weight:bold;
		  font-size:108%;
		  }

		  span.homograph-number {
		  font-family:sans-serif;
		  font-weight:bold;
		  font-size:50%;
		  vertical-align: sub;
		  }

		  div.note {
		  padding-right: 1em;
		  padding-left: 1em;
		  background-color: #eeeeee; /*light-gray*/
		  }

		  span.sense-number{
		  /*If a headword has multiple meanings or senses, the sense numbers
		  appear in the bold weight of the entry type, both accented and
		  neutralized by extra white space around them. */
		  padding-right: .25em;
		  font-weight:bold;
		  /*            font-size: 70%;*/ /*this should be the x height of the surrounding type*/
		  }

		  /*The parts of speech are abbreviated and expressed in italic type.*/
		  span.grammatical-info{
		  font-style: italic;
		  }

		  /*Plurals or related words appear in the medium weight of the sans serif
		  headword type; etymologies in combinations of small and regular capitals,
		  italic and roman. Simple icons indicate new parts of speech or sense,
		  and related words.*/


		  /*So quotations and samples from everyday speech are expressed in italic,
		  as is structural emphasis (parts of speech; subject areas; translations or
		  source words in another language); small capitals are used only to
		  indicate the century of a word’s origin, and related words.*/
		  span.example {
		  font-style:italic;
		  }

		  span.trait {
		  font-variant: small-caps;
		  }

		  span.translation {

		  }

		  span.pronunciation{
		  }

		  span.subentry span.headword{
			font-family:sans-serif; /*to do: select the sans-serif variety from writing system*/
			font-weight:bold;
			font-size:90%;

		  }
		  <!--
		  span.subentry {
			display: block;
			margin-left: 1em;
		  }
		  -->

		  <xsl:for-each select="document($writing-system-info-file)//WritingSystem">
		  span.<xsl:value-of select="Id"/> {
			font-family: "<xsl:value-of select="FontName"/>";
			font-size: <xsl:value-of select="FontSize"/>;
		  }
		  </xsl:for-each>

		</style>
	  </head>
	  <body>
		<xsl:apply-templates select="//entry"/>
	   </body>
	</html>
  </xsl:template>

  <xsl:template name="output-headword">
		<xsl:apply-templates select="field[@tag='headword']"/>
	<xsl:text> </xsl:text>
  </xsl:template>

  <xsl:template name="output-entry">
	<xsl:message/> <!-- for progress dialog-->
	<xsl:call-template name="output-headword"/>
	<xsl:apply-templates select="pronunciation"/>
	<xsl:apply-templates select="variant"/>
	<xsl:apply-templates select="trait"/>
	<xsl:apply-templates select="field[not(@tag='headword')]"/>
	<xsl:choose>
	  <xsl:when test="$entries-with-BaseForm-relation-rendered-as-subentries-of-base">
		<xsl:apply-templates select="relation[not(@name='BaseForm')]"/>
	  </xsl:when>
	  <xsl:otherwise>
		<xsl:apply-templates select="relation"/>
	  </xsl:otherwise>
	</xsl:choose>
	<xsl:apply-templates select="note"/>
	<xsl:apply-templates select="annotation"/>
	<xsl:choose>
	  <xsl:when test="$group-by-grammatical-info">
		<xsl:apply-templates select="sense[not(grammatical-info)]"/>
		<xsl:for-each select="sense">
		  <xsl:if test="not(preceding-sibling::sense/grammatical-info/@value =
					  grammatical-info/@value)">
			<xsl:apply-templates select="grammatical-info"/>
			<xsl:apply-templates select="parent::*/sense[grammatical-info/@value =current()/grammatical-info/@value]"/>
		  </xsl:if>
		</xsl:for-each>
	  </xsl:when>
	  <xsl:otherwise>
		<xsl:apply-templates select="sense"/>
	  </xsl:otherwise>
	</xsl:choose>

	<xsl:if test="$entries-with-BaseForm-relation-rendered-as-subentries-of-base">
	  <xsl:apply-templates select="//entry[descendant::relation[@name='BaseForm']/@ref=current()/@id]" mode="subentry"/>
	</xsl:if>
  </xsl:template>

  <xsl:template match="entry">
	<xsl:choose>
	  <xsl:when test="$entries-with-BaseForm-relation-rendered-as-subentries-of-base and
		relation[@name='BaseForm']">
		<div class="minorentry">
		  <xsl:call-template name="output-headword"/>
		  <span class="cross-reference">
			<span class="{$optionslist-writing-system}">
			  see <!-- TODO: localize from relations option list -->
			</span>
			<xsl:for-each select="//entry[@id=current()/descendant::relation[@name='BaseForm']/@ref]">
			  <xsl:call-template name="output-headword"/>
			</xsl:for-each>
		  </span>
		</div>
	  </xsl:when>
	  <xsl:otherwise>
		<div class="entry">
		  <xsl:call-template name="output-entry"/>
		</div>
	  </xsl:otherwise>
	</xsl:choose>
  </xsl:template>

  <xsl:template match="entry" mode="subentry">
	<span class="subentry">
	  <xsl:call-template name="output-entry"/>
	</span>
  </xsl:template>

  <!-- omit entries that have been deleted-->
  <xsl:template match="entry[@dateDeleted]"/>


  <xsl:template match="field[@tag='headword']">
	<span class="headword">
	  <xsl:apply-templates select="form[@lang=$headword-writing-system]"/>

	  <xsl:variable name="homograph-number">
		<xsl:value-of select="ancestor::entry/@order"/>
	  </xsl:variable>
	  <xsl:if test="$homograph-number != ''">
		<span class="homograph-number">
		  <xsl:value-of select="$homograph-number"/>
		</span>
	  </xsl:if>

	  <xsl:apply-templates select="form[not(@lang=$headword-writing-system)]"/>
	</span>
  </xsl:template>

  <xsl:template match="sense | subsense">
	<xsl:variable name="multiple-senses">
	  <xsl:choose>
		<xsl:when test="$group-by-grammatical-info">
		  <xsl:if test="not(grammatical-info) and count(parent::*/sense[not(grammatical-info)]) > 1">
			<xsl:value-of select="'yes'"/>
		  </xsl:if>
		  <xsl:if test="(count(parent::*/sense[grammatical-info/@value = current()/grammatical-info/@value]) > 1 or count(parent::*/subsense) > 1)">
			<xsl:value-of select="'yes'"/>
		  </xsl:if>
		</xsl:when>
		<xsl:otherwise>
		  <xsl:if test="(count(parent::*/sense) > 1 or count(parent::*/subsense) > 1)">
			<xsl:value-of select="'yes'"/>
		  </xsl:if>
		</xsl:otherwise>
	  </xsl:choose>
	</xsl:variable>

	<span class="sense">
	  <xsl:if test="$multiple-senses = 'yes'">
		<span class="sense-number">
		  <xsl:number level="multiple" count="sense | subsense" value="position()" format="1.1.1"/>
		</span>
	  </xsl:if>

	  <xsl:if test="not($group-by-grammatical-info)">
		<xsl:apply-templates select="grammatical-info"/>
	  </xsl:if>

	  <!-- if there is a definition, use that as the meaning; otherwise
			 use the gloss.-->
	  <xsl:choose>
		<xsl:when test="definition">
		  <xsl:apply-templates select="definition"/>
		</xsl:when>
		<xsl:otherwise>
		  <xsl:apply-templates select="gloss"/>
		</xsl:otherwise>
	  </xsl:choose>

	  <xsl:apply-templates select="example"/>
	  <xsl:apply-templates select="illustration"/>
	  <xsl:apply-templates select="trait"/>
	  <xsl:apply-templates select="field"/>
	  <xsl:apply-templates select="relation"/>
	  <xsl:apply-templates select="etymology"/>
	  <xsl:apply-templates select="note"/>
	  <xsl:apply-templates select="annotation"/>
	  <xsl:apply-templates select="subsense"/>
	  <xsl:text> </xsl:text>
	</span>
  </xsl:template>

  <xsl:template match="gloss">
	<span class="meaning">
	  <span class="{@lang}">
		<xsl:apply-templates/>
	  </span>
	</span>
	<xsl:if test="following-sibling::gloss">
	  <xsl:text>;</xsl:text>
	</xsl:if>
	<xsl:text> </xsl:text>
  </xsl:template>

  <xsl:template match="definition">
	<span class="meaning">
	  <xsl:apply-templates/>
	</span>
	<xsl:text> </xsl:text>
  </xsl:template>

  <xsl:template match="pronunciation">
	<span class="pronunciation">
	  <xsl:text>[</xsl:text>
	  <xsl:apply-templates/>
	  <xsl:text>]</xsl:text>
	</span>
	<xsl:text> </xsl:text>
  </xsl:template>

  <xsl:template match="example">
	<span class="example">
	  <xsl:apply-templates select="*[not(self::translation)]"/>
	</span>
	<xsl:text> </xsl:text>
	<xsl:apply-templates select="translation"/>
  </xsl:template>

  <xsl:template match="translation">
	<span class="translation">
	  <xsl:apply-templates/>
	</span>
	<xsl:text> </xsl:text>
  </xsl:template>

  <xsl:template match="note">
	<xsl:if test="$include-notes">
	  <div class="note">
		<xsl:apply-templates/>
	  </div>
	</xsl:if>
  </xsl:template>

  <xsl:template match="form">
	<span class="{@lang}">
	  <xsl:apply-templates select="text"/>
	</span>
	<xsl:if test="following-sibling::form">
	  <xsl:text> </xsl:text>
	</xsl:if>
  </xsl:template>

  <xsl:template match="span">
	<xsl:variable name="content">
	  <xsl:choose>
		<xsl:when test="@href">
		  <a href="{@href}">
			<xsl:apply-templates/>
		  </a>
		</xsl:when>
		<xsl:otherwise>
		  <xsl:apply-templates/>
		</xsl:otherwise>
	  </xsl:choose>
	</xsl:variable>

	<span>
	  <xsl:choose>
		<xsl:when test="@class and @lang">
		  <xsl:attribute name="class">
			<xsl:value-of select="@class"/>
		  </xsl:attribute>
		  <span class="{@lang}">
			<xsl:value-of select="$content"/>
		  </span>
		</xsl:when>
		<xsl:when test="@class">
		  <xsl:attribute name="class">
			<xsl:value-of select="@class"/>
		  </xsl:attribute>
		  <xsl:value-of select="$content"/>
		</xsl:when>
		<xsl:when test="@lang">
		  <xsl:attribute name="class">
			<xsl:value-of select="@lang"/>
		  </xsl:attribute>
		  <xsl:value-of select="$content"/>
		</xsl:when>
	  </xsl:choose>
	</span>
  </xsl:template>

  <xsl:template match="text()">
	<xsl:value-of select="normalize-space()"/>
  </xsl:template>

  <xsl:template match="grammatical-info">
	<!--
	we dropped, for example, the full point after part of speech
	abbreviations: its identity already having been underlined by
	the use of italic, by its abbreviation and by recurrence,
	there’s no need for a full point after n or vb.
	-->
	<span class="grammatical-info">
	  <span class="{$optionslist-writing-system}">
		<xsl:value-of select="@value"/>
	  </span>
	</span>
	<xsl:text> </xsl:text>
	<xsl:apply-templates select="trait"/>
  </xsl:template>

  <xsl:template match="trait">
	<span class="trait">
	  <span class="{@name}">
		<span class="{$optionslist-writing-system}">
		  <xsl:value-of select="@value"/>
		</span>
	  </span>
	</span>
	<xsl:text> </xsl:text>
  </xsl:template>
</xsl:transform>