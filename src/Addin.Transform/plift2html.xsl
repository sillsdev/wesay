<?xml version="1.0"?>
<xsl:transform xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

  <xsl:param name="optionslist-writing-system" select="'en'"/>

  <xsl:param name="headword-writing-system" select="//entry/field[@type='headword']/form/@lang"/>
  <xsl:param name="include-notes" select="false()"/>
  <xsl:param name="group-by-grammatical-info" select="false()"/>

  <xsl:variable name="entries-with-BaseForm-relation-rendered-as-subentries-of-base" select="true()"/>
  <xsl:variable name="link-to-usercss" select="false()"/>

  <xsl:template match="/">
	<html>
	  <head>
		<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />

	  </head>
	  <body>
		<xsl:apply-templates select="//entry"/>
	  </body>
	</html>
  </xsl:template>

  <xsl:template name="output-headword">
	<xsl:apply-templates select="field[@type='headword']"/>
	<xsl:text> </xsl:text>
  </xsl:template>

  <xsl:template name="output-entry">
	<xsl:message/>
	<!-- for progress dialog-->
	<xsl:call-template name="output-headword"/>
	<xsl:apply-templates select="pronunciation"/>
	<xsl:apply-templates select="variant"/>


	<xsl:apply-templates select="note"/>
	<xsl:apply-templates select="annotation"/>

	<span class="senses">
   <xsl:choose>
	  <xsl:when test="$group-by-grammatical-info">
		<xsl:apply-templates select="sense[not(field[@type='grammatical-info'])]"/>
		<xsl:for-each select="sense">
		  <xsl:if
			test="not(preceding-sibling::sense/field[@type='grammatical-info']/@value =
			field[@type='grammatical-info']/@value)">
			<xsl:apply-templates select="field[@type='grammatical-info']"/>
			<xsl:apply-templates
			  select="parent::*/sense[field[@type='grammatical-info']/@value =current()/field[@type='grammatical-info']/@value]"/>
		  </xsl:if>
		</xsl:for-each>
	  </xsl:when>
	  <xsl:otherwise>
		<xsl:apply-templates select="sense"/>
	 </xsl:otherwise>
	</xsl:choose>
</span>
	<xsl:choose>
	  <xsl:when test="$entries-with-BaseForm-relation-rendered-as-subentries-of-base">
		<xsl:apply-templates select="relation[not(@name='BaseForm')]"/>
	  </xsl:when>
	  <xsl:otherwise>
		<xsl:apply-templates select="relation"/>
	  </xsl:otherwise>
	</xsl:choose>

	<xsl:apply-templates select="trait"/>
	<xsl:apply-templates select="field[not(@type='headword')]"/>

	<xsl:if test="$entries-with-BaseForm-relation-rendered-as-subentries-of-base">
	  <xsl:apply-templates
		select="//entry[descendant::relation[@name='BaseForm']/@ref=current()/@id]" mode="subentry"
	  />
	</xsl:if>
  </xsl:template>



  <xsl:template match="entry">
	<xsl:choose>
	  <xsl:when
		test="$entries-with-BaseForm-relation-rendered-as-subentries-of-base and
		relation[@name='BaseForm']">
		<div class="minorentry">
		  <xsl:call-template name="output-headword"/>
		  <span class="cross-reference">
			<span class="{$optionslist-writing-system}"> see
			  <!-- TODO: localize from relations option list -->
			</span>
			<xsl:for-each
			  select="//entry[@id=current()/descendant::relation[@name='BaseForm']/@ref]">
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


  <!-- TODO: there is a @first='true' we can use to output, say, non-bold for the forms after the first one -->

  <xsl:template match="field[@type='headword']">
	<xsl:element name="span">
	  <xsl:attribute name="class">headword</xsl:attribute>
	  <xsl:attribute name="lang"><xsl:value-of select="form[@first]/@lang"/></xsl:attribute>

	  <!-- <xsl:apply-templates select="form[@lang=$headword-writing-system]"/> -->
	  <xsl:value-of select="form[@first]"/>

	 <!-- TODO: other headword writing systems
		<xsl:variable name="homograph-number">
		<xsl:value-of select="ancestor::entry/@order"/>
	  </xsl:variable>
	  <xsl:if test="$homograph-number != ''">
		<span class="homograph-number">
		  <xsl:value-of select="$homograph-number"/>
		</span>
	  </xsl:if>
	  <xsl:apply-templates select="form[not(@first)]"/>-->
	</xsl:element>
  </xsl:template>

  <xsl:template match="sense | subsense">
	<xsl:variable name="multiple-senses">
	  <xsl:choose>
		<xsl:when test="$group-by-grammatical-info">
		  <xsl:if test="not(field[@type='grammatical-info']) and count(parent::*/sense[not(field[@type='grammatical-info'])]) > 1">
			<xsl:value-of select="'yes'"/>
		  </xsl:if>
		  <xsl:if
			test="(count(parent::*/sense[field[@type='grammatical-info']/@value = current()/field[@type='grammatical-info']/@value]) > 1 or count(parent::*/subsense) > 1)">
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
		<xsl:apply-templates select="field[@type='grammatical-info']"/>
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

	  <xsl:choose>
		<xsl:when test="example">
		  <span class="examples">
		  <xsl:apply-templates select="example"/>
		  </span>
		</xsl:when>
	  </xsl:choose>
		 <xsl:apply-templates select="illustration"/>
	  <xsl:apply-templates select="trait"/>
	  <xsl:apply-templates select="field[not(@type='grammatical-info')]"/>
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


  <!-- e.g. <span class="definition" lang="en"><xitem><xlanguagetag>....  -->
  <xsl:template match="form" mode="wierdxItemThing">
	<xsl:element name="span">
	  <xsl:attribute name="class"><xsl:value-of select="name(parent::node())"/></xsl:attribute>
	  <xsl:attribute name="lang"><xsl:value-of select="@lang"/></xsl:attribute>
	  <xsl:element name="span">
		<xsl:attribute name="class">xitem</xsl:attribute>
		<xsl:attribute name="lang"><xsl:value-of select="@lang"/></xsl:attribute>
		<!-- I'm commenting this out because it's more work and, frankly, all the lang tags clutter up the page.
		  <xsl:element name="span">
			 <xsl:attribute name="class">xlanguagetag</xsl:attribute>
			<xsl:attribute name="lang">en</xsl:attribute> <! todo this should be language of the abbreviation ->

		  <xsl:value-of select="@lang"/> <!-todo should be the abbreviation of the ws... get it from the plift? ->
		</xsl:element>
		-->
		<xsl:value-of select="current()"/>
	  </xsl:element>
	</xsl:element>
  </xsl:template>

  <xsl:template match="definition">
	  <xsl:apply-templates mode="wierdxItemThing"></xsl:apply-templates>
	<xsl:text> </xsl:text>
  </xsl:template>

  <xsl:template match="illustration">
	<div class="illustration">
	  <xsl:element name="img">
		<!-- added this to help word, but it didn't work -->
		<xsl:attribute name="style">width:100; height:auto</xsl:attribute>
		<xsl:attribute name="src">
		  <xsl:text>../pictures/</xsl:text><xsl:value-of select="@href"/>
		</xsl:attribute>
		<xsl:attribute name="title">
		  <!-- leave it up to the plift process to filter down to the one form it wants -->
		  <xsl:value-of select="label/form/text"/>
		</xsl:attribute>
		<xsl:attribute name="width">100</xsl:attribute>
	   </xsl:element>
	</div>
	<!-- todo: get the label out as a caption -->
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
	<xsl:apply-templates mode="langAndForm" select="*[not(self::translation)]"></xsl:apply-templates>
   <!-- <span class="example">
	  <xsl:apply-templates select="*[not(self::translation)]"/>
	</span>
	<xsl:text> </xsl:text>-->


	<xsl:choose>
	  <xsl:when test="translation">
		<span class="translations">
		  <xsl:apply-templates mode="langAndForm" select="translation"></xsl:apply-templates>
		</span>
	  </xsl:when>
	</xsl:choose>
	<!--<xsl:apply-templates select="translation"/>-->
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
<!--
  <xsl:template match="form">
	<span class="{@lang}">
	  <xsl:apply-templates select="text"/>
	</span>
	<xsl:if test="following-sibling::form">
	  <xsl:text> </xsl:text>
	</xsl:if>
  </xsl:template>-->

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

<!-- e.g. <span class="definition" lang="en">blah blah</span>  -->
  <xsl:template match="form" mode="langAndForm">
	<xsl:element name="span">
	  <xsl:attribute name="class"><xsl:value-of select="name(parent::node())"/></xsl:attribute>
	  <xsl:attribute name="lang"><xsl:value-of select="@lang"/></xsl:attribute>
	<xsl:value-of select="current()"/>
	</xsl:element>
  </xsl:template>

  <!--
	Handle this:
	<relation name="confer" ref="ane_ID0EIJAG">
	  <field type="headword-of-target">
		<form lang="v">
		  <text>aneCitation</text>
		</form>
	  </field>
	  </relation>-->
  <xsl:template match="relation[field/@type='headword-of-target']"> <!-- where plift generator found the target for us -->
	<xsl:choose>
	  <xsl:when test="@name = 'confer'">
		<xsl:text>see</xsl:text>
	  </xsl:when>
	  <xsl:otherwise>
		<xsl:value-of select="@name"/>
	  </xsl:otherwise>
	</xsl:choose>
	<xsl:text> </xsl:text>
	<span class="relation-target">
	  <xsl:apply-templates select="field[@type='headword-of-target']"/>
	</span>
	<xsl:text> </xsl:text>
  </xsl:template>

  <xsl:template match="form" mode="partofspeech">
	  <xsl:element name="span">
		<xsl:attribute name="class">partofspeech</xsl:attribute>
		   <xsl:attribute name="lang"><xsl:value-of select="@lang"/></xsl:attribute>
		   <xsl:value-of select="current()"/>
	  </xsl:element>
	<xsl:text> </xsl:text>
  </xsl:template>


<!--  <xsl:template match="grammatical-info">
For plift, this is moved to a field, so that no lookup of the actual forms needed. -->
  <xsl:template match="field[@type='grammatical-info']">

	<span class="grammatical-info">
	  <xsl:apply-templates select="form" mode="partofspeech"></xsl:apply-templates>
	</span>

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
