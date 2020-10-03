import React from 'react';
import PropTypes from 'prop-types';
import Pagination from 'react-bootstrap-4-pagination';

// import Pagination from 'react-bootstrap/Pagination';

const PageNavigation = ({ pagination: { currentPage, itemsPerPage, totalItems, totalPages } }) => {
	console.log(currentPage);

	// let paginationConfig = {
	// 	totalPages: totalPages,
	// 	currentPage: currentPage,
	// 	showMax: itemsPerPage,
	// 	size: 'lg',
	// 	threeDots: true,
	// 	prevNext: true,
	// 	borderColor: 'red',
	// 	activeBorderColor: 'black',
	// 	activeBgColor: 'grey',
	// 	disabledBgColor: 'red',
	// 	activeColor: 'red',
	// 	color: 'purple',
	// 	disabledColor: 'green',
	// 	circle: true,
	// 	shadow: true
	// };

	return (
		<div>
			<h1>Hello</h1>
		</div>
	);
};

PageNavigation.propTypes = {};

export default PageNavigation;
